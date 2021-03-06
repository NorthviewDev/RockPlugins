// <copyright>
// Copyright Southeast Christian Church
//
// Licensed under the  Southeast Christian Church License (the "License");
// you may not use this file except in compliance with the License.
// A copy of the License shoud be included with this file.
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// </copyright>
//
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Linq;

using Rock.Attribute;
using Rock.CheckIn;
using Rock.Data;
using Rock.Model;
using Rock.Web.Cache;

namespace Rock.Workflow.Action.CheckIn
{
    /// <summary>
    /// Saves the selected check-in data as attendance and removes session credits
    /// </summary>
    [ActionCategory( "SECC > Check-In" )]
    [Description( "Saves the selected check-in data as attendance and removes 1 session" )]
    [Export( typeof( ActionComponent ) )]
    [ExportMetadata( "ComponentName", "Save Attendance And Remove Session" )]
    [AttributeField( Rock.SystemGuid.EntityType.GROUP, "Checkin Group Attribute", "Group attribute which contains the group to check-in against." )]
    [TextField( "Session Attribute Key", "Key name of the group which contains the session count.", true, "Sessions" )]
    public class SaveAttendanceAndRemoveSession : CheckInActionComponent
    {
        /// <summary>
        /// Executes the specified workflow.
        /// </summary>
        /// <param name="rockContext">The rock context.</param>
        /// <param name="action">The workflow action.</param>
        /// <param name="entity">The entity.</param>
        /// <param name="errorMessages">The error messages.</param>
        /// <returns></returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public override bool Execute( RockContext rockContext, Model.WorkflowAction action, Object entity, out List<string> errorMessages )
        {
            var checkInState = GetCheckInState( entity, out errorMessages );
            if ( checkInState != null )
            {
                Guid checkinGroupAttributeGuid = GetAttributeValue( action, "CheckinGroupAttribute" ).AsGuid();
                if ( checkinGroupAttributeGuid == Guid.Empty )
                {
                    throw new Exception( "CheckInGroupAttribute not set. Set attribute to continue." );
                }
                string checkinGroupAttributeKey = AttributeCache.Get( checkinGroupAttributeGuid ).Key;

                string sessionAttributeKey = GetAttributeValue( action, "SessionAttributeKey" );

                AttendanceCode attendanceCode = null;
                DateTime startDateTime = RockDateTime.Now;

                bool reuseCodeForFamily = checkInState.CheckInType != null && checkInState.CheckInType.ReuseSameCode;
                int securityCodeLength = checkInState.CheckInType != null ? checkInState.CheckInType.SecurityCodeAlphaNumericLength : 3;

                AttendanceCodeService attendanceCodeService = new AttendanceCodeService( rockContext );
                AttendanceService attendanceService = new AttendanceService( rockContext );
                GroupMemberService groupMemberService = new GroupMemberService( rockContext );
                PersonAliasService personAliasService = new PersonAliasService( rockContext );
                GroupService groupService = new GroupService( rockContext );

                var family = checkInState.CheckIn.CurrentFamily;
                if ( family != null )
                {
                    foreach ( var person in family.GetPeople( true ) )
                    {
                        if ( reuseCodeForFamily && attendanceCode != null )
                        {
                            person.SecurityCode = attendanceCode.Code;
                        }
                        else
                        {
                            attendanceCode = AttendanceCodeService.GetNew( securityCodeLength );
                            person.SecurityCode = attendanceCode.Code;
                        }

                        foreach ( var groupType in person.GetGroupTypes( true ) )
                        {
                            foreach ( var group in groupType.GetGroups( true ) )
                            {
                                var referenceGroupGuid = group.Group.GetAttributeValue( checkinGroupAttributeKey ).AsGuid();
                                var referenceGroup = groupService.Get( referenceGroupGuid );
                                if ( referenceGroup == null )
                                {
                                    group.Selected = false;
                                    continue;
                                }
                                GroupMember groupMember = groupMemberService.GetByGroupIdAndPersonId( referenceGroup.Id, person.Person.Id ).FirstOrDefault();
                                if ( groupMember == null )
                                {
                                    group.Selected = false;
                                    continue;
                                }
                                groupMember.LoadAttributes();
                                int sessions = groupMember.GetAttributeValue( sessionAttributeKey ).AsInteger();

                                foreach ( var location in group.GetLocations( true ) )
                                {
                                    foreach ( var schedule in location.GetSchedules( true ) )
                                    {
                                        if ( sessions == 0 )
                                        {
                                            continue;
                                        }

                                        // Only create one attendance record per day for each person/schedule/group/location
                                        var attendance = attendanceService.Get( startDateTime, location.Location.Id, schedule.Schedule.Id, group.Group.Id, person.Person.Id );
                                        if ( attendance == null )
                                        {
                                            var primaryAlias = personAliasService.GetPrimaryAlias( person.Person.Id );
                                            if ( primaryAlias != null )
                                            {
                                                attendance = attendanceService.AddOrUpdate( primaryAlias.Id, startDateTime.Date, group.Group.Id,
                                                    location.Location.Id, schedule.Schedule.Id, location.CampusId,
                                                    checkInState.Kiosk.Device.Id, checkInState.CheckIn.SearchType.Id,
                                                    checkInState.CheckIn.SearchValue, family.Group.Id, attendanceCode.Id );

                                                attendanceService.Add( attendance );
                                            }

                                            //decrement sessions and save
                                            sessions--;
                                            groupMember.SetAttributeValue( sessionAttributeKey, sessions );
                                            groupMember.SaveAttributeValues();
                                        }
                                        else
                                        {
                                            foreach ( var cPerson in checkInState.CheckIn.Families.SelectMany( f => f.People ) )
                                            {
                                                cPerson.Selected = false;
                                                cPerson.GroupTypes.ForEach( gt => gt.Selected = false );
                                            }
                                            return true;
                                        }

                                        attendance.AttendanceCodeId = attendanceCode.Id;
                                        attendance.StartDateTime = startDateTime;
                                        attendance.EndDateTime = null;
                                        attendance.DidAttend = true;

                                        KioskLocationAttendance.AddAttendance( attendance );
                                    }
                                }
                            }
                        }
                    }
                }

                rockContext.SaveChanges();
                return true;
            }

            errorMessages.Add( $"Attempted to run {this.GetType().GetFriendlyTypeName()} in check-in, but the check-in state was null." );
            return false;
        }
    }
}