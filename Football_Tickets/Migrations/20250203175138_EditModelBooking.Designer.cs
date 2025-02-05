﻿// <auto-generated />
using System;
using Football_Tickets.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace Football_Tickets.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20250203175138_EditModelBooking")]
    partial class EditModelBooking
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.11")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("Football_Tickets.Models.ApplicationUser", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<int>("AccessFailedCount")
                        .HasColumnType("int");

                    b.Property<string>("Address")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ConcurrencyStamp")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Email")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("EmailConfirmed")
                        .HasColumnType("bit");

                    b.Property<bool>("LockoutEnabled")
                        .HasColumnType("bit");

                    b.Property<DateTimeOffset?>("LockoutEnd")
                        .HasColumnType("datetimeoffset");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("NormalizedEmail")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("NormalizedUserName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PasswordHash")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PhoneNumber")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("PhoneNumberConfirmed")
                        .HasColumnType("bit");

                    b.Property<string>("SecurityStamp")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("TwoFactorEnabled")
                        .HasColumnType("bit");

                    b.Property<string>("UserName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("photo")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("Football_Tickets.Models.Booking", b =>
                {
                    b.Property<int>("BookingId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("BookingID");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("BookingId"));

                    b.Property<string>("ApplicationUserId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<DateTime>("Date")
                        .HasColumnType("datetime");

                    b.Property<int>("MatchId")
                        .HasColumnType("int")
                        .HasColumnName("MatchID");

                    b.Property<int>("TicketId")
                        .HasColumnType("int")
                        .HasColumnName("TicketID");

                    b.HasKey("BookingId")
                        .HasName("PK__Booking__73951ACDFFB44174");

                    b.HasIndex("ApplicationUserId");

                    b.HasIndex("MatchId");

                    b.HasIndex("TicketId");

                    b.ToTable("Booking", (string)null);
                });

            modelBuilder.Entity("Football_Tickets.Models.Cart", b =>
                {
                    b.Property<string>("ApplicationUserId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<int>("MatchId")
                        .HasColumnType("int");

                    b.Property<int>("Count")
                        .HasColumnType("int");

                    b.Property<int?>("SeatNumber")
                        .HasColumnType("int");

                    b.Property<DateTime>("Time")
                        .HasColumnType("datetime2");

                    b.Property<string>("section")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("ApplicationUserId", "MatchId");

                    b.HasIndex("MatchId");

                    b.ToTable("Carts");
                });

            modelBuilder.Entity("Football_Tickets.Models.Match", b =>
                {
                    b.Property<int>("MatchId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("MatchID");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("MatchId"));

                    b.Property<int?>("AwayTeamId")
                        .HasColumnType("int")
                        .HasColumnName("AwayTeamID");

                    b.Property<int?>("HomeTeamId")
                        .HasColumnType("int")
                        .HasColumnName("HomeTeamID");

                    b.Property<DateTime>("MatchDate")
                        .HasColumnType("datetime");

                    b.Property<decimal?>("Section1Price")
                        .IsRequired()
                        .HasColumnType("decimal(10, 2)");

                    b.Property<decimal?>("Section2Price")
                        .IsRequired()
                        .HasColumnType("decimal(10, 2)");

                    b.Property<decimal?>("Section3Price")
                        .IsRequired()
                        .HasColumnType("decimal(10, 2)");

                    b.Property<int?>("StadiumId")
                        .HasColumnType("int")
                        .HasColumnName("StadiumID");

                    b.HasKey("MatchId")
                        .HasName("PK__Matches__4218C837AAD0359E");

                    b.HasIndex("AwayTeamId");

                    b.HasIndex("HomeTeamId");

                    b.HasIndex("StadiumId");

                    b.ToTable("Matches");
                });

            modelBuilder.Entity("Football_Tickets.Models.Payment", b =>
                {
                    b.Property<int>("PaymentId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("PaymentID");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("PaymentId"));

                    b.Property<decimal?>("Amount")
                        .HasColumnType("decimal(10, 2)");

                    b.Property<DateTime>("Date")
                        .HasColumnType("datetime");

                    b.Property<string>("Method")
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<int>("TicketId")
                        .HasColumnType("int")
                        .HasColumnName("TicketID");

                    b.HasKey("PaymentId")
                        .HasName("PK__Payment__9B556A58C3FF029A");

                    b.HasIndex("TicketId");

                    b.ToTable("Payment", (string)null);
                });

            modelBuilder.Entity("Football_Tickets.Models.Season", b =>
                {
                    b.Property<int>("SeasonId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("SeasonID");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("SeasonId"));

                    b.Property<decimal>("BasePrice")
                        .HasColumnType("decimal(10, 2)");

                    b.Property<DateOnly>("EndDate")
                        .HasColumnType("date");

                    b.Property<string>("SeasonName")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<DateOnly>("StartDate")
                        .HasColumnType("date");

                    b.HasKey("SeasonId")
                        .HasName("PK__Seasons__C1814E18A73DB4D1");

                    b.ToTable("Seasons");
                });

            modelBuilder.Entity("Football_Tickets.Models.SeasonTicket", b =>
                {
                    b.Property<int>("SeasonTicketId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("SeasonTicketID");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("SeasonTicketId"));

                    b.Property<int>("SeasonId")
                        .HasColumnType("int")
                        .HasColumnName("SeasonID");

                    b.Property<int>("SectionId")
                        .HasColumnType("int")
                        .HasColumnName("SectionID");

                    b.Property<int>("TeamId")
                        .HasColumnType("int")
                        .HasColumnName("TeamID");

                    b.Property<decimal?>("TotalPrice")
                        .HasColumnType("decimal(10, 2)");

                    b.HasKey("SeasonTicketId")
                        .HasName("PK__SeasonTi__0F72A0F2F0196C36");

                    b.HasIndex("SeasonId");

                    b.HasIndex("SectionId");

                    b.HasIndex("TeamId");

                    b.ToTable("SeasonTickets");
                });

            modelBuilder.Entity("Football_Tickets.Models.Section", b =>
                {
                    b.Property<int>("SectionId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("SectionID");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("SectionId"));

                    b.Property<int?>("Capacity")
                        .HasColumnType("int");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<int>("StadiumId")
                        .HasColumnType("int")
                        .HasColumnName("StadiumID");

                    b.HasKey("SectionId")
                        .HasName("PK__Sections__80EF08923EE5E6CC");

                    b.HasIndex("StadiumId");

                    b.ToTable("Sections");
                });

            modelBuilder.Entity("Football_Tickets.Models.Stadium", b =>
                {
                    b.Property<int>("StadiumId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("StadiumID");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("StadiumId"));

                    b.Property<int?>("Capacity")
                        .IsRequired()
                        .HasColumnType("int");

                    b.Property<string>("Location")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<int?>("SelectedTeamId")
                        .HasColumnType("int");

                    b.HasKey("StadiumId")
                        .HasName("PK__Stadiums__ED8330384A445346");

                    b.ToTable("Stadiums");
                });

            modelBuilder.Entity("Football_Tickets.Models.Team", b =>
                {
                    b.Property<int>("TeamId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("TeamID");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("TeamId"));

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("LogoUrl")
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("LogoURL");

                    b.Property<int?>("StadiumId")
                        .HasColumnType("int");

                    b.Property<string>("TeamName")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.HasKey("TeamId")
                        .HasName("PK__Teams__123AE7B990B6685E");

                    b.HasIndex("StadiumId");

                    b.ToTable("Teams");
                });

            modelBuilder.Entity("Football_Tickets.Models.Ticket", b =>
                {
                    b.Property<int>("TicketId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("TicketID");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("TicketId"));

                    b.Property<int>("MatchId")
                        .HasColumnType("int")
                        .HasColumnName("MatchID");

                    b.Property<int?>("Seatnumber")
                        .HasColumnType("int");

                    b.Property<int>("SectionId")
                        .HasColumnType("int")
                        .HasColumnName("SectionID");

                    b.Property<decimal?>("SectionPrice")
                        .HasColumnType("decimal(10, 2)");

                    b.Property<string>("Sections")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("StadiumId")
                        .HasColumnType("int")
                        .HasColumnName("StadiumID");

                    b.HasKey("TicketId")
                        .HasName("PK__Tickets__712CC627D49670AE");

                    b.HasIndex("MatchId");

                    b.HasIndex("SectionId");

                    b.HasIndex("StadiumId");

                    b.ToTable("Tickets");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRole", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("ConcurrencyStamp")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("NormalizedName")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Roles");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("ClaimType")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ClaimValue")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("RoleId")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("RoleClaims");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("ClaimType")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ClaimValue")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UserId")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("UserClaims");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.Property<string>("LoginProvider")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("ProviderKey")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("ProviderDisplayName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UserId")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("LoginProvider", "ProviderKey");

                    b.ToTable("UserLogins");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.Property<string>("UserId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("RoleId")
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("UserId", "RoleId");

                    b.ToTable("UserRoles");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.Property<string>("UserId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("LoginProvider")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Value")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("UserId", "LoginProvider", "Name");

                    b.ToTable("UserTokens");
                });

            modelBuilder.Entity("Football_Tickets.Models.Booking", b =>
                {
                    b.HasOne("Football_Tickets.Models.ApplicationUser", "ApplicationUser")
                        .WithMany()
                        .HasForeignKey("ApplicationUserId");

                    b.HasOne("Football_Tickets.Models.Match", "Match")
                        .WithMany("Bookings")
                        .HasForeignKey("MatchId")
                        .IsRequired()
                        .HasConstraintName("FK__Booking__MatchID__4E88ABD4");

                    b.HasOne("Football_Tickets.Models.Ticket", "Ticket")
                        .WithMany("Bookings")
                        .HasForeignKey("TicketId")
                        .IsRequired()
                        .HasConstraintName("FK__Booking__TicketI__4F7CD00D");

                    b.Navigation("ApplicationUser");

                    b.Navigation("Match");

                    b.Navigation("Ticket");
                });

            modelBuilder.Entity("Football_Tickets.Models.Cart", b =>
                {
                    b.HasOne("Football_Tickets.Models.ApplicationUser", "ApplicationUser")
                        .WithMany()
                        .HasForeignKey("ApplicationUserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Football_Tickets.Models.Match", "Match")
                        .WithMany()
                        .HasForeignKey("MatchId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("ApplicationUser");

                    b.Navigation("Match");
                });

            modelBuilder.Entity("Football_Tickets.Models.Match", b =>
                {
                    b.HasOne("Football_Tickets.Models.Team", "AwayTeam")
                        .WithMany("MatchAwayTeams")
                        .HasForeignKey("AwayTeamId")
                        .HasConstraintName("FK__Matches__AwayTea__3C69FB99");

                    b.HasOne("Football_Tickets.Models.Team", "HomeTeam")
                        .WithMany("MatchHomeTeams")
                        .HasForeignKey("HomeTeamId")
                        .HasConstraintName("FK__Matches__HomeTea__3B75D760");

                    b.HasOne("Football_Tickets.Models.Stadium", "Stadium")
                        .WithMany("Matches")
                        .HasForeignKey("StadiumId")
                        .HasConstraintName("FK__Matches__Stadium__3D5E1FD2");

                    b.Navigation("AwayTeam");

                    b.Navigation("HomeTeam");

                    b.Navigation("Stadium");
                });

            modelBuilder.Entity("Football_Tickets.Models.Payment", b =>
                {
                    b.HasOne("Football_Tickets.Models.Ticket", "Ticket")
                        .WithMany("Payments")
                        .HasForeignKey("TicketId")
                        .IsRequired()
                        .HasConstraintName("FK__Payment__TicketI__52593CB8");

                    b.Navigation("Ticket");
                });

            modelBuilder.Entity("Football_Tickets.Models.SeasonTicket", b =>
                {
                    b.HasOne("Football_Tickets.Models.Season", "Season")
                        .WithMany("SeasonTickets")
                        .HasForeignKey("SeasonId")
                        .IsRequired()
                        .HasConstraintName("FK__SeasonTic__Seaso__45F365D3");

                    b.HasOne("Football_Tickets.Models.Section", "Section")
                        .WithMany("SeasonTickets")
                        .HasForeignKey("SectionId")
                        .IsRequired()
                        .HasConstraintName("FK__SeasonTic__Secti__46E78A0C");

                    b.HasOne("Football_Tickets.Models.Team", "Team")
                        .WithMany("SeasonTickets")
                        .HasForeignKey("TeamId")
                        .IsRequired()
                        .HasConstraintName("FK__SeasonTic__TeamI__44FF419A");

                    b.Navigation("Season");

                    b.Navigation("Section");

                    b.Navigation("Team");
                });

            modelBuilder.Entity("Football_Tickets.Models.Section", b =>
                {
                    b.HasOne("Football_Tickets.Models.Stadium", "Stadium")
                        .WithMany("Sections")
                        .HasForeignKey("StadiumId")
                        .IsRequired()
                        .HasConstraintName("FK__Sections__Stadiu__403A8C7D");

                    b.Navigation("Stadium");
                });

            modelBuilder.Entity("Football_Tickets.Models.Team", b =>
                {
                    b.HasOne("Football_Tickets.Models.Stadium", "Stadium")
                        .WithMany("Teams")
                        .HasForeignKey("StadiumId");

                    b.Navigation("Stadium");
                });

            modelBuilder.Entity("Football_Tickets.Models.Ticket", b =>
                {
                    b.HasOne("Football_Tickets.Models.Match", "Match")
                        .WithMany("Tickets")
                        .HasForeignKey("MatchId")
                        .IsRequired()
                        .HasConstraintName("FK__Tickets__MatchID__49C3F6B7");

                    b.HasOne("Football_Tickets.Models.Section", "Section")
                        .WithMany("Tickets")
                        .HasForeignKey("SectionId")
                        .IsRequired()
                        .HasConstraintName("FK__Tickets__Section__4BAC3F29");

                    b.HasOne("Football_Tickets.Models.Stadium", "Stadium")
                        .WithMany("Tickets")
                        .HasForeignKey("StadiumId")
                        .HasConstraintName("FK__Tickets__Stadium__4AB81AF0");

                    b.Navigation("Match");

                    b.Navigation("Section");

                    b.Navigation("Stadium");
                });

            modelBuilder.Entity("Football_Tickets.Models.Match", b =>
                {
                    b.Navigation("Bookings");

                    b.Navigation("Tickets");
                });

            modelBuilder.Entity("Football_Tickets.Models.Season", b =>
                {
                    b.Navigation("SeasonTickets");
                });

            modelBuilder.Entity("Football_Tickets.Models.Section", b =>
                {
                    b.Navigation("SeasonTickets");

                    b.Navigation("Tickets");
                });

            modelBuilder.Entity("Football_Tickets.Models.Stadium", b =>
                {
                    b.Navigation("Matches");

                    b.Navigation("Sections");

                    b.Navigation("Teams");

                    b.Navigation("Tickets");
                });

            modelBuilder.Entity("Football_Tickets.Models.Team", b =>
                {
                    b.Navigation("MatchAwayTeams");

                    b.Navigation("MatchHomeTeams");

                    b.Navigation("SeasonTickets");
                });

            modelBuilder.Entity("Football_Tickets.Models.Ticket", b =>
                {
                    b.Navigation("Bookings");

                    b.Navigation("Payments");
                });
#pragma warning restore 612, 618
        }
    }
}
