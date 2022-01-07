using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API.Migrations
{
    public partial class AddedTriggerForOutOfStock2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"CREATE TRIGGER ADD_TO_OUTOFSTOCK_AFTER_UPDATE ON dbo.Products

            AFTER UPDATE
            AS
                BEGIN

            declare @prevValue int, @newValue int, @productId bigint

            select @prevValue = deleted.Quantity from deleted

            select @newValue = inserted.Quantity from inserted

            select @productId = inserted.Id from inserted


            if @newValue < 1

            begin
                update Products set Quantity = 0 where Id = @productId

            if @prevValue > 0

            begin
                insert into OutOfStocks values(@productId, GETDATE())


            end
                end
            END");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP TRIGGER ADD_TO_OUTOFSTOCK_AFTER_UPDATE");
        }
    }
}
