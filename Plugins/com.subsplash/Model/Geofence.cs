/* 
 * Push API
 *
 * No description provided (generated by Swagger Codegen https://github.com/swagger-api/swagger-codegen)
 *
 * OpenAPI spec version: 1.0.0
 * Contact: api@subsplash.com
 * Generated by: https://github.com/swagger-api/swagger-codegen.git
 */
using System;
using System.Linq;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.ComponentModel.DataAnnotations;

namespace com.subsplash.Model
{
    /// <summary>
    /// Geofence
    /// </summary>
    [DataContract]
        public partial class Geofence :  IEquatable<Geofence>, IValidatableObject
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Geofence" /> class.
        /// </summary>
        /// <param name="id">Unique identifier of geofence object..</param>
        /// <param name="appKey">Six character app code..</param>
        /// <param name="createdAt">Moment when the geofence was created..</param>
        /// <param name="updatedAt">Moment when the geofence was updated..</param>
        /// <param name="deletedAt">Moment when the geofence was deleted..</param>
        /// <param name="expiredAt">Moment when the geofence expired..</param>
        /// <param name="radius">The radius of the geofence..</param>
        /// <param name="repeats">Indicates whether the geofence repeats.</param>
        /// <param name="type">Type of the geofence.</param>
        /// <param name="notifyOnEntry">Indicates whether to notify on entry.</param>
        /// <param name="notifyOnExit">Indicates whether to notify on exit.</param>
        /// <param name="notifyOnDwellInside">Indicates whether to notify on dwell inside.</param>
        /// <param name="notifyOnDwellOutside">Indicates whether to notify on dwell outside.</param>
        /// <param name="embedded">embedded.</param>
        /// <param name="links">links.</param>
        public Geofence(Guid? id = default(Guid?), string appKey = default(string), DateTime? createdAt = default(DateTime?), DateTime? updatedAt = default(DateTime?), DateTime? deletedAt = default(DateTime?), DateTime? expiredAt = default(DateTime?), decimal? radius = default(decimal?), bool? repeats = default(bool?), string type = default(string), bool? notifyOnEntry = default(bool?), bool? notifyOnExit = default(bool?), bool? notifyOnDwellInside = default(bool?), bool? notifyOnDwellOutside = default(bool?), GeofenceEmbedded embedded = default(GeofenceEmbedded), SelfLink links = default(SelfLink))
        {
            this.Id = id;
            this.AppKey = appKey;
            this.CreatedAt = createdAt;
            this.UpdatedAt = updatedAt;
            this.DeletedAt = deletedAt;
            this.ExpiredAt = expiredAt;
            this.Radius = radius;
            this.Repeats = repeats;
            this.Type = type;
            this.NotifyOnEntry = notifyOnEntry;
            this.NotifyOnExit = notifyOnExit;
            this.NotifyOnDwellInside = notifyOnDwellInside;
            this.NotifyOnDwellOutside = notifyOnDwellOutside;
            this.Embedded = embedded;
            this.Links = links;
        }
        
        /// <summary>
        /// Unique identifier of geofence object.
        /// </summary>
        /// <value>Unique identifier of geofence object.</value>
        [DataMember(Name="id", EmitDefaultValue=false)]
        public Guid? Id { get; set; }

        /// <summary>
        /// Six character app code.
        /// </summary>
        /// <value>Six character app code.</value>
        [DataMember(Name="app_key", EmitDefaultValue=false)]
        public string AppKey { get; set; }

        /// <summary>
        /// Moment when the geofence was created.
        /// </summary>
        /// <value>Moment when the geofence was created.</value>
        [DataMember(Name="created_at", EmitDefaultValue=false)]
        public DateTime? CreatedAt { get; set; }

        /// <summary>
        /// Moment when the geofence was updated.
        /// </summary>
        /// <value>Moment when the geofence was updated.</value>
        [DataMember(Name="updated_at", EmitDefaultValue=false)]
        public DateTime? UpdatedAt { get; set; }

        /// <summary>
        /// Moment when the geofence was deleted.
        /// </summary>
        /// <value>Moment when the geofence was deleted.</value>
        [DataMember(Name="deleted_at", EmitDefaultValue=false)]
        public DateTime? DeletedAt { get; set; }

        /// <summary>
        /// Moment when the geofence expired.
        /// </summary>
        /// <value>Moment when the geofence expired.</value>
        [DataMember(Name="expired_at", EmitDefaultValue=false)]
        public DateTime? ExpiredAt { get; set; }

        /// <summary>
        /// The radius of the geofence.
        /// </summary>
        /// <value>The radius of the geofence.</value>
        [DataMember(Name="radius", EmitDefaultValue=false)]
        public decimal? Radius { get; set; }

        /// <summary>
        /// Indicates whether the geofence repeats
        /// </summary>
        /// <value>Indicates whether the geofence repeats</value>
        [DataMember(Name="repeats", EmitDefaultValue=false)]
        public bool? Repeats { get; set; }

        /// <summary>
        /// Type of the geofence
        /// </summary>
        /// <value>Type of the geofence</value>
        [DataMember(Name="type", EmitDefaultValue=false)]
        public string Type { get; set; }

        /// <summary>
        /// Indicates whether to notify on entry
        /// </summary>
        /// <value>Indicates whether to notify on entry</value>
        [DataMember(Name="notify_on_entry", EmitDefaultValue=false)]
        public bool? NotifyOnEntry { get; set; }

        /// <summary>
        /// Indicates whether to notify on exit
        /// </summary>
        /// <value>Indicates whether to notify on exit</value>
        [DataMember(Name="notify_on_exit", EmitDefaultValue=false)]
        public bool? NotifyOnExit { get; set; }

        /// <summary>
        /// Indicates whether to notify on dwell inside
        /// </summary>
        /// <value>Indicates whether to notify on dwell inside</value>
        [DataMember(Name="notify_on_dwell_inside", EmitDefaultValue=false)]
        public bool? NotifyOnDwellInside { get; set; }

        /// <summary>
        /// Indicates whether to notify on dwell outside
        /// </summary>
        /// <value>Indicates whether to notify on dwell outside</value>
        [DataMember(Name="notify_on_dwell_outside", EmitDefaultValue=false)]
        public bool? NotifyOnDwellOutside { get; set; }

        /// <summary>
        /// Gets or Sets Embedded
        /// </summary>
        [DataMember(Name="_embedded", EmitDefaultValue=false)]
        public GeofenceEmbedded Embedded { get; set; }

        /// <summary>
        /// Gets or Sets Links
        /// </summary>
        [DataMember(Name="_links", EmitDefaultValue=false)]
        public SelfLink Links { get; set; }

        /// <summary>
        /// Returns the string presentation of the object
        /// </summary>
        /// <returns>String presentation of the object</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("class Geofence {\n");
            sb.Append("  Id: ").Append(Id).Append("\n");
            sb.Append("  AppKey: ").Append(AppKey).Append("\n");
            sb.Append("  CreatedAt: ").Append(CreatedAt).Append("\n");
            sb.Append("  UpdatedAt: ").Append(UpdatedAt).Append("\n");
            sb.Append("  DeletedAt: ").Append(DeletedAt).Append("\n");
            sb.Append("  ExpiredAt: ").Append(ExpiredAt).Append("\n");
            sb.Append("  Radius: ").Append(Radius).Append("\n");
            sb.Append("  Repeats: ").Append(Repeats).Append("\n");
            sb.Append("  Type: ").Append(Type).Append("\n");
            sb.Append("  NotifyOnEntry: ").Append(NotifyOnEntry).Append("\n");
            sb.Append("  NotifyOnExit: ").Append(NotifyOnExit).Append("\n");
            sb.Append("  NotifyOnDwellInside: ").Append(NotifyOnDwellInside).Append("\n");
            sb.Append("  NotifyOnDwellOutside: ").Append(NotifyOnDwellOutside).Append("\n");
            sb.Append("  Embedded: ").Append(Embedded).Append("\n");
            sb.Append("  Links: ").Append(Links).Append("\n");
            sb.Append("}\n");
            return sb.ToString();
        }
  
        /// <summary>
        /// Returns the JSON string presentation of the object
        /// </summary>
        /// <returns>JSON string presentation of the object</returns>
        public virtual string ToJson()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }

        /// <summary>
        /// Returns true if objects are equal
        /// </summary>
        /// <param name="input">Object to be compared</param>
        /// <returns>Boolean</returns>
        public override bool Equals(object input)
        {
            return this.Equals(input as Geofence);
        }

        /// <summary>
        /// Returns true if Geofence instances are equal
        /// </summary>
        /// <param name="input">Instance of Geofence to be compared</param>
        /// <returns>Boolean</returns>
        public bool Equals(Geofence input)
        {
            if (input == null)
                return false;

            return 
                (
                    this.Id == input.Id ||
                    (this.Id != null &&
                    this.Id.Equals(input.Id))
                ) && 
                (
                    this.AppKey == input.AppKey ||
                    (this.AppKey != null &&
                    this.AppKey.Equals(input.AppKey))
                ) && 
                (
                    this.CreatedAt == input.CreatedAt ||
                    (this.CreatedAt != null &&
                    this.CreatedAt.Equals(input.CreatedAt))
                ) && 
                (
                    this.UpdatedAt == input.UpdatedAt ||
                    (this.UpdatedAt != null &&
                    this.UpdatedAt.Equals(input.UpdatedAt))
                ) && 
                (
                    this.DeletedAt == input.DeletedAt ||
                    (this.DeletedAt != null &&
                    this.DeletedAt.Equals(input.DeletedAt))
                ) && 
                (
                    this.ExpiredAt == input.ExpiredAt ||
                    (this.ExpiredAt != null &&
                    this.ExpiredAt.Equals(input.ExpiredAt))
                ) && 
                (
                    this.Radius == input.Radius ||
                    (this.Radius != null &&
                    this.Radius.Equals(input.Radius))
                ) && 
                (
                    this.Repeats == input.Repeats ||
                    (this.Repeats != null &&
                    this.Repeats.Equals(input.Repeats))
                ) && 
                (
                    this.Type == input.Type ||
                    (this.Type != null &&
                    this.Type.Equals(input.Type))
                ) && 
                (
                    this.NotifyOnEntry == input.NotifyOnEntry ||
                    (this.NotifyOnEntry != null &&
                    this.NotifyOnEntry.Equals(input.NotifyOnEntry))
                ) && 
                (
                    this.NotifyOnExit == input.NotifyOnExit ||
                    (this.NotifyOnExit != null &&
                    this.NotifyOnExit.Equals(input.NotifyOnExit))
                ) && 
                (
                    this.NotifyOnDwellInside == input.NotifyOnDwellInside ||
                    (this.NotifyOnDwellInside != null &&
                    this.NotifyOnDwellInside.Equals(input.NotifyOnDwellInside))
                ) && 
                (
                    this.NotifyOnDwellOutside == input.NotifyOnDwellOutside ||
                    (this.NotifyOnDwellOutside != null &&
                    this.NotifyOnDwellOutside.Equals(input.NotifyOnDwellOutside))
                ) && 
                (
                    this.Embedded == input.Embedded ||
                    (this.Embedded != null &&
                    this.Embedded.Equals(input.Embedded))
                ) && 
                (
                    this.Links == input.Links ||
                    (this.Links != null &&
                    this.Links.Equals(input.Links))
                );
        }

        /// <summary>
        /// Gets the hash code
        /// </summary>
        /// <returns>Hash code</returns>
        public override int GetHashCode()
        {
            unchecked // Overflow is fine, just wrap
            {
                int hashCode = 41;
                if (this.Id != null)
                    hashCode = hashCode * 59 + this.Id.GetHashCode();
                if (this.AppKey != null)
                    hashCode = hashCode * 59 + this.AppKey.GetHashCode();
                if (this.CreatedAt != null)
                    hashCode = hashCode * 59 + this.CreatedAt.GetHashCode();
                if (this.UpdatedAt != null)
                    hashCode = hashCode * 59 + this.UpdatedAt.GetHashCode();
                if (this.DeletedAt != null)
                    hashCode = hashCode * 59 + this.DeletedAt.GetHashCode();
                if (this.ExpiredAt != null)
                    hashCode = hashCode * 59 + this.ExpiredAt.GetHashCode();
                if (this.Radius != null)
                    hashCode = hashCode * 59 + this.Radius.GetHashCode();
                if (this.Repeats != null)
                    hashCode = hashCode * 59 + this.Repeats.GetHashCode();
                if (this.Type != null)
                    hashCode = hashCode * 59 + this.Type.GetHashCode();
                if (this.NotifyOnEntry != null)
                    hashCode = hashCode * 59 + this.NotifyOnEntry.GetHashCode();
                if (this.NotifyOnExit != null)
                    hashCode = hashCode * 59 + this.NotifyOnExit.GetHashCode();
                if (this.NotifyOnDwellInside != null)
                    hashCode = hashCode * 59 + this.NotifyOnDwellInside.GetHashCode();
                if (this.NotifyOnDwellOutside != null)
                    hashCode = hashCode * 59 + this.NotifyOnDwellOutside.GetHashCode();
                if (this.Embedded != null)
                    hashCode = hashCode * 59 + this.Embedded.GetHashCode();
                if (this.Links != null)
                    hashCode = hashCode * 59 + this.Links.GetHashCode();
                return hashCode;
            }
        }

        /// <summary>
        /// To validate all properties of the instance
        /// </summary>
        /// <param name="validationContext">Validation context</param>
        /// <returns>Validation Result</returns>
        IEnumerable<System.ComponentModel.DataAnnotations.ValidationResult> IValidatableObject.Validate(ValidationContext validationContext)
        {
            yield break;
        }
    }
}
