﻿using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BlueBerry.ToysShop.Web.Migrations
{
    /// <inheritdoc />
    public partial class AddPublishDatewithProduct : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "PublishDate",
                table: "Products",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PublishDate",
                table: "Products");
        }
    }
}
