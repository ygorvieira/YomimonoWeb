using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Yomimono.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddAuthorEntityAndBookStatus : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // 1. Create Authors table first (needed for data migration FK)
            migrationBuilder.CreateTable(
                name: "Authors",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Authors", x => x.Id);
                });

            // 2. Create BookAuthors table
            migrationBuilder.CreateTable(
                name: "BookAuthors",
                columns: table => new
                {
                    BookId = table.Column<Guid>(type: "uuid", nullable: false),
                    AuthorId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BookAuthors", x => new { x.BookId, x.AuthorId });
                    table.ForeignKey(
                        name: "FK_BookAuthors_Authors_AuthorId",
                        column: x => x.AuthorId,
                        principalTable: "Authors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BookAuthors_Books_BookId",
                        column: x => x.BookId,
                        principalTable: "Books",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            // 3. Add new columns as nullable first
            migrationBuilder.AddColumn<Guid>(
                name: "GenreId",
                table: "Books",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsLiked",
                table: "Books",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "ReadingStatus",
                table: "Books",
                type: "character varying(20)",
                maxLength: 20,
                nullable: true);

            // 4. Data migration: create Authors from existing data
            //    and assign default GenreId to existing books
            migrationBuilder.Sql("""
                DO $$
                DECLARE
                    book_rec RECORD;
                    author_id UUID;
                    first_genre_id UUID;
                BEGIN
                    -- Create Author entities from distinct Book.Author strings
                    FOR book_rec IN SELECT DISTINCT "Author" FROM "Books" WHERE "Author" IS NOT NULL AND "Author" != '' LOOP
                        author_id := gen_random_uuid();
                        INSERT INTO "Authors" ("Id", "Name", "CreatedAt", "UpdatedAt", "DeletedAt")
                        VALUES (author_id, book_rec."Author", NOW(), NOW(), NULL);
                    END LOOP;

                    -- Create BookAuthor links for existing books
                    FOR book_rec IN SELECT "Id", "Author" FROM "Books" WHERE "Author" IS NOT NULL AND "Author" != '' LOOP
                        INSERT INTO "BookAuthors" ("BookId", "AuthorId")
                        SELECT book_rec."Id", a."Id" FROM "Authors" a WHERE a."Name" = book_rec."Author";
                    END LOOP;

                    -- Assign a default GenreId for existing books
                    SELECT "Id" INTO first_genre_id FROM "Genres" LIMIT 1;
                    IF first_genre_id IS NULL THEN
                        author_id := gen_random_uuid();
                        INSERT INTO "Genres" ("Id", "Name", "CreatedAt", "UpdatedAt", "DeletedAt")
                        VALUES (author_id, 'Migrado', NOW(), NOW(), NULL);
                        first_genre_id := author_id;
                    END IF;
                    UPDATE "Books" SET "GenreId" = first_genre_id WHERE "GenreId" IS NULL;
                END $$;
                """);

            // 5. Make GenreId NOT NULL
            migrationBuilder.AlterColumn<Guid>(
                name: "GenreId",
                table: "Books",
                type: "uuid",
                nullable: false,
                oldClrType: typeof(Guid),
                oldNullable: true);

            // 6. Drop old string columns
            migrationBuilder.DropColumn(
                name: "Author",
                table: "Books");

            migrationBuilder.DropColumn(
                name: "Genre",
                table: "Books");

            // 7. Create indexes and FKs
            migrationBuilder.CreateIndex(
                name: "IX_Authors_Name",
                table: "Authors",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_BookAuthors_AuthorId",
                table: "BookAuthors",
                column: "AuthorId");

            migrationBuilder.CreateIndex(
                name: "IX_Books_GenreId",
                table: "Books",
                column: "GenreId");

            migrationBuilder.AddForeignKey(
                name: "FK_Books_Genres_GenreId",
                table: "Books",
                column: "GenreId",
                principalTable: "Genres",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Books_Genres_GenreId",
                table: "Books");

            // Re-add old string columns
            migrationBuilder.AddColumn<string>(
                name: "Author",
                table: "Books",
                type: "character varying(150)",
                maxLength: 150,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Genre",
                table: "Books",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            // Restore old Author and Genre data
            migrationBuilder.Sql("""
                UPDATE "Books" b
                SET "Author" = COALESCE((SELECT a."Name" FROM "BookAuthors" ba JOIN "Authors" a ON a."Id" = ba."AuthorId" WHERE ba."BookId" = b."Id" LIMIT 1), ''),
                    "Genre" = COALESCE((SELECT g."Name" FROM "Genres" g WHERE g."Id" = b."GenreId"), '')
                """);

            // Make GenreId nullable before dropping
            migrationBuilder.AlterColumn<Guid>(
                name: "GenreId",
                table: "Books",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid));

            migrationBuilder.DropColumn(
                name: "GenreId",
                table: "Books");

            migrationBuilder.DropColumn(
                name: "IsLiked",
                table: "Books");

            migrationBuilder.DropColumn(
                name: "ReadingStatus",
                table: "Books");

            migrationBuilder.DropTable(
                name: "BookAuthors");

            migrationBuilder.DropTable(
                name: "Authors");

            migrationBuilder.DropIndex(
                name: "IX_Books_GenreId",
                table: "Books");
        }
    }
}
