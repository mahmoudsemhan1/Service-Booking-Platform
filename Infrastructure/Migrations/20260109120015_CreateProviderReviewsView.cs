using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class CreateProviderReviewsView : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
            EXEC('CREATE VIEW vw_ProviderReviews AS
            SELECT 
                r.Id, 
                r.ProviderId, 
                r.Rating, 
                r.Comment, 
                r.CreatedAt,
                r.IsDeleted,
                u.FullName AS CustomerName,
                s.Title AS ServiceName
            FROM Reviews r
            INNER JOIN AspNetUsers u ON r.UserId = u.Id
            INNER JOIN Bookings b ON r.BookingId = b.Id
            INNER JOIN Services s ON b.ServiceId = s.Id')");

        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP VIEW vw_ProviderReviews");
        }
    }
}
