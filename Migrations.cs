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
                .Column("Year", DbType.Int32)
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

            ContentDefinitionManager.AlterPartDefinition(
                typeof(BookingPart).Name, cfg => cfg.Attachable());

            ContentDefinitionManager.AlterTypeDefinition("Booking", cfg => cfg
               .WithPart(typeof(BookingPart).Name)
               );

            ContentDefinitionManager.AlterPartDefinition(
                typeof(SeasonPart).Name, cfg => cfg.Attachable());

            ContentDefinitionManager.AlterTypeDefinition("Season", cfg => cfg
               .WithPart(typeof(SeasonPart).Name)
               );

            return 1;
        }

        //public int UpdateFrom1()
        //{

        //    return 2;
        //}

        //public int UpdateFrom2()
        //{

        //    return 3;
        //}
    }

}
