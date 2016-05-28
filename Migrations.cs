using Cascade.Booking.Models;
using Orchard.ContentManagement.MetaData;
using Orchard.Core.Contents.Extensions;
using Orchard.Data.Migration;
using System;
using System.Data;

namespace Cascade.Booking
{
    public class Migrations : DataMigrationImpl
    {

        public int Create()
        {
            // Creating table BookingRecord
            SchemaBuilder.CreateTable("BookingRecord", table => table
                .ContentPartRecord()
                .Column("Name", DbType.String)
                .Column("RawGuests", DbType.String, c => c.Unlimited())
                //.Column("Year", DbType.Int32)
                .Column("BookingState", DbType.String)
            );

            // Creating table SeasonRecord
            SchemaBuilder.CreateTable("SeasonRecord", table => table
                .ContentPartRecord()
                .Column("Title", DbType.String)
                .Column<DateTime>("FromDate", column => column.Nullable()) // don't call it 'From' nHibernate blows up
                .Column<DateTime>("ToDate", column => column.Nullable())  // don't call it 'To' nHibernate blows up
                .Column("Rate", DbType.Decimal)
            );

            ContentDefinitionManager.AlterPartDefinition(typeof(BookingPart).Name, cfg => cfg
                .WithDescription("Represents a season (with a daily Rate) within which a booking can be made.")
                .Attachable());

            ContentDefinitionManager.AlterTypeDefinition("Booking", cfg => cfg
                .WithSetting("Description", "Represents a booking and a list of guests.")
                .WithPart(typeof(BookingPart).Name)
                .WithPart("IdentityPart")
                .WithPart("CommonPart")
               );

            ContentDefinitionManager.AlterPartDefinition(typeof(SeasonPart).Name, cfg => cfg
                .WithDescription("A booking and a list of guests.")
                .Attachable());

            ContentDefinitionManager.AlterTypeDefinition("Season", cfg => cfg
                .WithSetting("Description", "Represents a season (with a daily Rate) within which a booking can be made.")
                .WithPart(typeof(SeasonPart).Name)
                .WithPart("IdentityPart")
               );

            return 5;
        }

        public int UpdateFrom1()
        {
            ContentDefinitionManager.AlterTypeDefinition("Booking", cfg => cfg.WithPart("IdentityPart"));
            ContentDefinitionManager.AlterTypeDefinition("Season", cfg => cfg.WithPart("IdentityPart"));

            return 2;
        }

        public int UpdateFrom2()
        {
            ContentDefinitionManager.AlterTypeDefinition("Booking", cfg => cfg
                .WithSetting("Description", "Represents a booking and a list of guests."));
            ContentDefinitionManager.AlterTypeDefinition("Season", cfg => cfg
                .WithSetting("Description", "Represents a season (with a daily Rate) within which a booking can be made."));

            return 3;
        }

        public int UpdateFrom3()
        {
            ContentDefinitionManager.AlterPartDefinition(typeof(BookingPart).Name, cfg => cfg
                .WithDescription("Represents a season (with a daily Rate) within which a booking can be made.")
                );
            ContentDefinitionManager.AlterPartDefinition(typeof(SeasonPart).Name, cfg => cfg
                .WithDescription("A booking and a list of guests.")
                );

            return 4;
        }

        public int UpdateFrom4()
        {
            ContentDefinitionManager.AlterTypeDefinition(typeof(BookingPart).Name, cfg => cfg
                .WithPart("CommonPart")
                );

            return 5;
        }
    }

}
