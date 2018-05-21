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
// <copyright>
// Copyright by the Spark Development Network
//
// Licensed under the Rock Community License (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.rockrms.com/license
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// </copyright>
//
using System;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Web.UI;

using Rock;
using Rock.Attribute;
using Rock.Data;
using Rock.Model;
using Rock.Web.UI;
using Rock.Web.Cache;
using System.Web.UI.WebControls;
using System.Collections.Generic;
using OfficeOpenXml;
using System.IO;
using System.Drawing;

namespace RockWeb.Blocks.Reporting.NextGen
{
    [DisplayName( "Medication Dispense" )]
    [Category( "SECC > Reporting > NextGen" )]
    [Description( "Tool for noting when medications should be given out." )]
    [DefinedTypeField( "Medication Schedule Defined Type", "Defined type which contain the values for the possible times to give medication.", key: "DefinedType" )]
    [GroupField( "Group", "Group of people to track medication despensment." )]
    [TextField( "Medication Matrix Key", "The attribute key for the medication schedule matrix." )]
    [CategoryField( "History Category", "Category to save the history.", false, "Rock.Model.History" )]
    [TextField( "Group Member Attribute Filter", "Group member filter to sort group by.", false )]
    public partial class MedicationDispense : RockBlock
    {
        List<MedicalItem> medicalItems = new List<MedicalItem>();
        protected override void OnLoad( EventArgs e )
        {
            nbAlert.Visible = false;

            if ( !Page.IsPostBack )
            {
                dpDate.SelectedDate = RockDateTime.Today;
                var definedType = DefinedTypeCache.Read( GetAttributeValue( "DefinedType" ).AsGuid() );
                if ( definedType != null )
                {
                    ddlSchedule.DataSource = definedType.DefinedValues;
                    ddlSchedule.DataBind();
                    ddlSchedule.Items.Insert( 0, new ListItem( "", "" ) );
                }

                Rock.Model.Attribute filterAttribute = null;

                var filterAttributeKey = GetAttributeValue( "GroupMemberAttributeFilter" );
                if ( !string.IsNullOrWhiteSpace( filterAttributeKey ) )
                {
                    RockContext rockContext = new RockContext();
                    GroupService groupService = new GroupService( rockContext );
                    AttributeService attributeService = new AttributeService( rockContext );
                    var group = groupService.Get( GetAttributeValue( "Group" ).AsGuid() );
                    if ( group == null )
                    {
                        nbAlert.Visible = true;
                        nbAlert.Text = "Group not found";
                        return;
                    }
                    var groupId = group.Id.ToString();
                    var groupTypeId = group.GroupTypeId.ToString();
                    var groupEntityid = EntityTypeCache.GetId<Rock.Model.GroupMember>();

                    filterAttribute = attributeService.Queryable()
                        .Where( a =>
                        ( a.EntityTypeQualifierValue == groupId || a.EntityTypeQualifierValue == groupTypeId )
                        && a.Key == filterAttributeKey
                        && a.EntityTypeId == groupEntityid )
                    .FirstOrDefault();

                    if ( filterAttribute != null )
                    {
                        AttributeValueService attributeValueService = new AttributeValueService( rockContext );
                        ddlAttribute.Label = filterAttribute.Name;
                        var qry = new GroupMemberService( rockContext ).Queryable().
                            Where( gm => gm.GroupId == group.Id )
                            .Join(
                                attributeValueService.Queryable().Where( av => av.AttributeId == filterAttribute.Id ),
                                m => m.Id,
                                av => av.EntityId,
                                ( m, av ) => new { Key = av.Value, Value = av.Value } )
                                .DistinctBy( a => a.Key )
                                .OrderBy( a => a.Key )
                                .ToList();
                        ddlAttribute.DataSource = qry;
                        ddlAttribute.DataBind();
                        ddlAttribute.Items.Insert( 0, new ListItem( "", "" ) );
                    }
                    else
                    {
                        pnlAttribute.Visible = false;
                    }
                }
                else
                {
                    pnlAttribute.Visible = false;
                }
                BindGrid();
            }

            gGrid.Actions.ShowExcelExport = false;
            gGrid.Actions.ShowMergeTemplate = false;

            if ( this.ContextEntity() == null )
            {
                LinkButton excel = new LinkButton()
                {
                    ID = "btnExcel",
                    Text = "<i class='fa fa-table'></i>",
                    CssClass = "btn btn-default btn-sm"
                };
                gGrid.Actions.Controls.Add( excel );
                excel.Click += GenerateExcel;
                ScriptManager.GetCurrent( this.Page ).RegisterPostBackControl( excel );
            }
        }



        private void BindGrid()
        {
            gGrid.DataSource = GetMedicalItems();
            ;
            gGrid.DataBind();

            if ( !dpDate.SelectedDate.HasValue
                || dpDate.SelectedDate.Value != Rock.RockDateTime.Today )
            {
                gGrid.Columns[gGrid.Columns.Count - 1].Visible = false;
            }
            else
            {
                gGrid.Columns[gGrid.Columns.Count - 1].Visible = true;
            }
        }

        private List<MedicalItem> GetMedicalItems()
        {
            RockContext rockContext = new RockContext();
            GroupService groupService = new GroupService( rockContext );
            var group = groupService.Get( GetAttributeValue( "Group" ).AsGuid() );
            if ( group == null )
            {
                nbAlert.Visible = true;
                nbAlert.Text = "Group not found";
                return null;
            }
            var groupId = group.Id.ToString();
            var groupTypeId = group.GroupTypeId.ToString();
            var groupEntityid = EntityTypeCache.GetId<Rock.Model.GroupMember>();
            var key = GetAttributeValue( "MedicationMatrixKey" );

            AttributeService attributeService = new AttributeService( rockContext );
            Rock.Model.Attribute attribute = attributeService.Queryable()
                .Where( a =>
                    ( a.EntityTypeQualifierValue == groupId || a.EntityTypeQualifierValue == groupTypeId )
                    && a.Key == key
                    && a.EntityTypeId == groupEntityid )
                .FirstOrDefault();

            if ( attribute == null )
            {
                nbAlert.Visible = true;
                nbAlert.Text = "Medication attribute not found";
                return null;
            }

            Rock.Model.Attribute filterAttribute = null;
            var filterAttributeKey = GetAttributeValue( "GroupMemberAttributeFilter" );
            if ( !string.IsNullOrWhiteSpace( filterAttributeKey ) )
            {
                filterAttribute = attributeService.Queryable()
                    .Where( a =>
                    ( a.EntityTypeQualifierValue == groupId || a.EntityTypeQualifierValue == groupTypeId )
                    && a.Key == filterAttributeKey
                    && a.EntityTypeId == groupEntityid )
                .FirstOrDefault();
            }

            var attributeMatrixItemEntityId = EntityTypeCache.GetId<AttributeMatrixItem>();

            AttributeValueService attributeValueService = new AttributeValueService( rockContext );
            AttributeMatrixService attributeMatrixService = new AttributeMatrixService( rockContext );
            AttributeMatrixItemService attributeMatrixItemService = new AttributeMatrixItemService( rockContext );
            HistoryService historyService = new HistoryService( rockContext );

            var qry = new GroupMemberService( rockContext ).Queryable().
                Where( gm => gm.GroupId == group.Id )
                .Join(
                    attributeValueService.Queryable().Where( av => av.AttributeId == attribute.Id ),
                    m => m.Id,
                    av => av.EntityId,
                    ( m, av ) => new { Member = m, AttributeValue = av.Value }
                )
                .Join(
                    attributeMatrixService.Queryable(),
                    m => m.AttributeValue,
                    am => am.Guid.ToString(),
                    ( m, am ) => new { Member = m.Member, AttributeMatrix = am }
                )
                .Join(
                    attributeMatrixItemService.Queryable(),
                    m => m.AttributeMatrix.Id,
                    ami => ami.AttributeMatrixId,
                    ( m, ami ) => new { Member = m.Member, AttributeMatrixItem = ami, TemplateId = ami.AttributeMatrix.AttributeMatrixTemplateId }
                )
                .Join(
                    attributeService.Queryable().Where( a => a.EntityTypeId == attributeMatrixItemEntityId ),
                    m => m.TemplateId.ToString(),
                    a => a.EntityTypeQualifierValue,
                    ( m, a ) => new { Member = m.Member, AttributeMatrixItem = m.AttributeMatrixItem, Attribute = a }
                )
                .Join(
                    attributeValueService.Queryable(),
                    m => new { EntityId = m.AttributeMatrixItem.Id, AttributeId = m.Attribute.Id },
                    av => new { EntityId = av.EntityId ?? 0, AttributeId = av.AttributeId },
                    ( m, av ) => new { Member = m.Member, Attribute = m.Attribute, AttributeValue = av, MatrixItemId = m.AttributeMatrixItem.Id, FilterValue = "" }
                );

            if ( filterAttribute != null && pnlAttribute.Visible && !string.IsNullOrWhiteSpace( ddlAttribute.SelectedValue ) )
            {
                var filterValue = ddlAttribute.SelectedValue;
                qry = qry
                    .Join(
                    attributeValueService.Queryable().Where( av => av.AttributeId == filterAttribute.Id ),
                    m => new { Id = m.Member.Id, Value = filterValue },
                    av => new { Id = av.EntityId ?? 0, Value = av.Value },
                    ( m, av ) => new { Member = m.Member, Attribute = m.Attribute, AttributeValue = m.AttributeValue, MatrixItemId = m.MatrixItemId, FilterValue = av.Value } );
            }

            var members = qry.GroupBy( a => a.Member )
                .ToList();

            var firstDay = ( dpDate.SelectedDate ?? Rock.RockDateTime.Today ).Date;
            var nextday = firstDay.AddDays( 1 );

            var personIds = members.Select( m => m.Key.PersonId );
            var attributeMatrixEntityTypeId = EntityTypeCache.GetId<AttributeMatrixItem>().Value;

            var historyItems = historyService.Queryable()
                .Where( h => personIds.Contains( h.EntityId ) )
                .Where( h => h.RelatedEntityTypeId == attributeMatrixEntityTypeId )
                .Where( h => h.CreatedDateTime >= firstDay && h.CreatedDateTime < nextday )
                .ToList();

            foreach ( var member in members )
            {
                if ( !string.IsNullOrWhiteSpace( tbName.Text )
                    && !member.Key.Person.FullName.ToLower().Contains( tbName.Text.ToLower() )
                    && !member.Key.Person.FullNameReversed.ToLower().Contains( tbName.Text.ToLower() ) )
                {
                    continue;
                }

                var medicines = member.GroupBy( m => m.MatrixItemId );
                foreach ( var medicine in medicines )
                {
                    var scheduleAtt = medicine.FirstOrDefault( m => m.Attribute.Key == "Schedule" );
                    if ( ddlSchedule.SelectedValue != "" && ddlSchedule.SelectedValue.AsGuid() != scheduleAtt.AttributeValue.Value.AsGuid() )
                    {
                        continue;
                    }

                    var medicalItem = new MedicalItem()
                    {
                        Person = member.Key.Person.FullNameReversed,
                        GroupMemberId = member.Key.Id,
                        GroupMember = member.FirstOrDefault().Member,
                        PersonId = member.Key.Person.Id,
                        FilterAttribute = member.FirstOrDefault().FilterValue

                    };

                    if ( scheduleAtt != null )
                    {
                        medicalItem.Schedule = DefinedValueCache.Read( scheduleAtt.AttributeValue.Value.AsGuid() ).Value;
                    }

                    var medAtt = medicine.FirstOrDefault( m => m.Attribute.Key == "Medication" );
                    if ( medAtt != null )
                    {
                        medicalItem.Medication = medAtt.AttributeValue.Value;
                    }

                    var instructionAtt = medicine.FirstOrDefault( m => m.Attribute.Key == "Instructions" );
                    if ( instructionAtt != null )
                    {
                        medicalItem.Instructions = instructionAtt.AttributeValue.Value;
                    }
                    medicalItem.Key = string.Format( "{0}|{1}", medicalItem.PersonId, medicine.Key );

                    var history = historyItems.Where( h => h.EntityId == medicalItem.PersonId && h.RelatedEntityId == medicine.Key );
                    if ( history.Any() )
                    {
                        medicalItem.Distributed = true;
                        medicalItem.History = string.Join( "<br>", history.Select( h => h.Summary ) );
                    }
                    medicalItems.Add( medicalItem );

                }
            }
            return medicalItems;
        }

        private void GenerateExcel( object sender, EventArgs e )
        {
            var medicalItems = GetMedicalItems();

            string filename = gGrid.ExportFilename;
            string workSheetName = "List";
            string title = "Medication Information";

            ExcelPackage excel = new ExcelPackage();
            excel.Workbook.Properties.Title = title;

            // add author info
            Rock.Model.UserLogin userLogin = Rock.Model.UserLoginService.GetCurrentUser();
            if ( userLogin != null )
            {
                excel.Workbook.Properties.Author = userLogin.Person.FullName;
            }
            else
            {
                excel.Workbook.Properties.Author = "Rock";
            }

            // add the page that created this
            excel.Workbook.Properties.SetCustomPropertyValue( "Source", this.Page.Request.Url.OriginalString );

            ExcelWorksheet worksheet = excel.Workbook.Worksheets.Add( workSheetName );
            worksheet.PrinterSettings.LeftMargin = .5m;
            worksheet.PrinterSettings.RightMargin = .5m;
            worksheet.PrinterSettings.TopMargin = .5m;
            worksheet.PrinterSettings.BottomMargin = .5m;

            //// write data to worksheet there are three supported data sources
            //// DataTables, DataViews and ILists

            int rowCounter = 4;
            int columnCounter = 0;

            List<string> columns = new List<string>() { "Name", "Medication", "Instructions", "Schedule" };
            var filterAttribute = "";
            var hasFilter = false;
            if ( !string.IsNullOrWhiteSpace( ddlAttribute.Label ) )
            {
                hasFilter = true;
                columns.Add( ddlAttribute.Label );
                filterAttribute = GetAttributeValue( "GroupMemberAttributeFilter" );
            }

            // print headings
            foreach ( String column in columns )
            {
                columnCounter++;
                worksheet.Cells[3, columnCounter].Value = column.SplitCase();
            }

            foreach ( var item in medicalItems )
            {
                SetExcelValue( worksheet.Cells[rowCounter, 1], item.Person );
                SetExcelValue( worksheet.Cells[rowCounter, 2], item.Medication );
                SetExcelValue( worksheet.Cells[rowCounter, 3], item.Instructions );
                SetExcelValue( worksheet.Cells[rowCounter, 4], item.Schedule );
                if ( hasFilter )
                {
                    item.GroupMember.LoadAttributes();
                    SetExcelValue( worksheet.Cells[rowCounter, 5], item.GroupMember.GetAttributeValue( filterAttribute ) );
                }
                rowCounter++;
            }

            var range = worksheet.Cells[3, 1, rowCounter, columnCounter];

            var table = worksheet.Tables.Add( range, "table1" );

            // ensure each column in the table has a unique name
            var columnNames = worksheet.Cells[3, 1, 3, columnCounter].Select( a => new { OrigColumnName = a.Text, Cell = a } ).ToList();
            columnNames.Reverse();
            foreach ( var col in columnNames )
            {
                int duplicateSuffix = 0;
                string uniqueName = col.OrigColumnName;

                // increment the suffix by 1 until there is only one column with that name
                while ( columnNames.Where( a => a.Cell.Text == uniqueName ).Count() > 1 )
                {
                    duplicateSuffix++;
                    uniqueName = col.OrigColumnName + duplicateSuffix.ToString();
                    col.Cell.Value = uniqueName;
                }
            }

            table.ShowFilter = true;
            table.TableStyle = OfficeOpenXml.Table.TableStyles.None;

            // format header range
            using ( ExcelRange r = worksheet.Cells[3, 1, 3, columnCounter] )
            {
                r.Style.Font.Bold = true;
                r.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                r.Style.Fill.BackgroundColor.SetColor( Color.FromArgb( 223, 223, 223 ) );
                r.Style.Font.Color.SetColor( Color.Black );
                r.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
            }

            // format and set title
            worksheet.Cells[1, 1].Value = title;
            using ( ExcelRange r = worksheet.Cells[1, 1, 2, columnCounter] )
            {
                r.Merge = true;
                r.Style.Font.SetFromFont( new Font( "Calibri", 22, FontStyle.Regular ) );
                r.Style.Font.Color.SetColor( Color.White );
                r.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                r.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                r.Style.Fill.BackgroundColor.SetColor( Color.FromArgb( 34, 41, 55 ) );

                // set border
                r.Style.Border.Left.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                r.Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                r.Style.Border.Top.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                r.Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
            }


            // freeze panes
            worksheet.View.FreezePanes( 3, 1 );

            // autofit columns for all cells
            worksheet.Cells.AutoFitColumns( 1000 );

            byte[] byteArray;
            using ( MemoryStream ms = new MemoryStream() )
            {
                excel.SaveAs( ms );
                byteArray = ms.ToArray();
            }

            // send the spreadsheet to the browser
            this.Page.EnableViewState = false;
            this.Page.Response.Clear();
            this.Page.Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            this.Page.Response.AppendHeader( "Content-Disposition", "attachment; filename=" + filename );

            this.Page.Response.Charset = string.Empty;
            this.Page.Response.BinaryWrite( byteArray );
            this.Page.Response.Flush();
            this.Page.Response.End();
        }

        private void SetExcelValue( ExcelRange range, object exportValue )
        {
            if ( exportValue != null &&
                ( exportValue is decimal || exportValue is decimal? ||
                exportValue is int || exportValue is int? ||
                exportValue is double || exportValue is double? ||
                exportValue is DateTime || exportValue is DateTime? ) )
            {
                range.Value = exportValue;
            }
            else
            {
                string value = exportValue != null ? exportValue.ToString().ConvertBrToCrLf().Replace( "&nbsp;", " " ) : string.Empty;
                range.Value = value;
                if ( value.Contains( Environment.NewLine ) )
                {
                    range.Style.WrapText = true;
                }
            }
        }

        class MedicalItem
        {
            public string Key { get; set; }
            public int PersonId { get; set; }
            public int GroupMemberId { get; set; }
            public GroupMember GroupMember { get; set; }
            public string Person { get; set; }
            public string Medication { get; set; }
            public string Instructions { get; set; }
            public string Schedule { get; set; }
            public bool Distributed { get; set; }
            public string History { get; set; }
            public string FilterAttribute { get; set; }
        }

        protected void Distribute_Click( object sender, Rock.Web.UI.Controls.RowEventArgs e )
        {
            RockContext rockContext = new RockContext();
            HistoryService historyService = new HistoryService( rockContext );
            var keys = ( ( string ) e.RowKeyValue ).SplitDelimitedValues();
            var personId = keys[0].AsInteger();
            var matrixId = keys[1].AsInteger();

            AttributeMatrixItemService attributeMatrixItemService = new AttributeMatrixItemService( rockContext );
            var matrix = attributeMatrixItemService.Get( matrixId );
            matrix.LoadAttributes();
            var category = new CategoryService( rockContext ).Get( GetAttributeValue( "HistoryCategory" ).AsGuid() );

            History history = new History()
            {
                CategoryId = category.Id,
                EntityTypeId = EntityTypeCache.GetId<Person>().Value,
                EntityId = personId,
                RelatedEntityTypeId = EntityTypeCache.GetId<AttributeMatrixItem>().Value,
                RelatedEntityId = matrixId,
                Verb = "Distributed",
                Caption = "Medication Distributed",
                Summary = string.Format( "<span class=\"field-name\">{0}</span> was distributed at <span class=\"field-name\">{1}</span>", matrix.GetAttributeValue( "Medication" ), Rock.RockDateTime.Now )
            };
            historyService.Add( history );
            rockContext.SaveChanges();
            BindGrid();
        }

        protected void ddlSchedule_SelectedIndexChanged( object sender, EventArgs e )
        {
            BindGrid();
        }

        protected void dpDate_TextChanged( object sender, EventArgs e )
        {
            BindGrid();
        }

        protected void tbName_TextChanged( object sender, EventArgs e )
        {
            BindGrid();
        }

        protected void ddlAttribute_SelectedIndexChanged( object sender, EventArgs e )
        {
            BindGrid();
        }
    }
}