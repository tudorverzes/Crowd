using System.Collections.Generic;
 using api.model;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace api.data;

public class ApplicationDbContext(DbContextOptions options) : IdentityDbContext<AppUser>(options)
{
	public DbSet<Event> Events { get; set; }
	public DbSet<TicketType> TicketTypes { get; set; }
	public DbSet<Ticket> Tickets { get; set; }
	public DbSet<Permission> Permissions { get; set; }
	public DbSet<Report> Reports { get; set; }
	public DbSet<Ad> Ads { get; set; }
	
	protected override void OnModelCreating(ModelBuilder builder)
	{
		base.OnModelCreating(builder);
		
		// Event entity
		builder.Entity<Event>()
			.HasKey(e => e.Id);
		builder.Entity<Event>()
			.HasMany(e => e.TicketTypes)
			.WithOne()
			.HasForeignKey(t => t.EventId)
			.OnDelete(DeleteBehavior.Cascade);
		builder.Entity<Event>()
			.HasMany(e => e.Tickets)
			.WithOne()
			.HasForeignKey(t => t.EventId)
			.OnDelete(DeleteBehavior.Cascade);
		builder.Entity<Event>()
			.HasMany(e => e.Permissions)
			.WithOne(p => p.Event)
			.HasForeignKey(p => p.EventId)
			.OnDelete(DeleteBehavior.Cascade);
		builder.Entity<Event>()
			.HasMany(e => e.Reports)
			.WithOne()
			.HasForeignKey(r => r.EventId)
			.OnDelete(DeleteBehavior.Cascade);
		
		// Permission entity
		builder.Entity<Permission>(x => x.HasKey(p => new { p.AppUserId, p.EventId }));
		builder.Entity<Permission>()
			.HasOne(p => p.AppUser)
			.WithMany(u => u.Permissions)
			.HasForeignKey(p => p.AppUserId);
		
		// Ad entity
		builder.Entity<Ad>()
			.HasKey(a => a.Id);
		builder.Entity<Ad>()
			.HasOne(a => a.Owner)
			.WithMany()
			.HasForeignKey(a => a.OwnerId);
		builder.Entity<Ad>()
			.HasMany(a => a.Targets)
			.WithOne(t => t.Ad)
			.HasForeignKey(t => t.AdId)
			.OnDelete(DeleteBehavior.Cascade);
		builder.Entity<Ad>()
			.HasMany(a => a.Keywords)
			.WithOne(k => k.Ad)
			.HasForeignKey(k => k.AdId)
			.OnDelete(DeleteBehavior.Cascade);
		
		builder.Entity<AdTarget>()
			.HasKey(t => t.Id);
		builder.Entity<AdTarget>()
			.HasDiscriminator<string>("TargetType")
			.HasValue<GeoRadiusTarget>("GeoRadius")
			.HasValue<SpecificEventTarget>("SpecificEvent");
		
		builder.Entity<GeoRadiusTarget>()
			.Property(g => g.Geometry)
			.HasColumnName("GeoRadiusTarget_Geometry")
			.HasColumnType("geography");
		
		builder.Entity<SpecificEventTarget>()
			.HasOne(s => s.Event)
			.WithMany()
			.HasForeignKey(s => s.EventId);
		
		builder.Entity<Ticket>()
			.HasKey(t => new { t.EventId, t.UniqueCode });
		
		builder.Entity<Event>()
			.OwnsOne(e => e.Location, locationBuilder =>
			{
				locationBuilder.Property(l => l.VenueName)
					.HasColumnName("Location_VenueName");
				locationBuilder.Property(l => l.AddressLine)
					.HasColumnName("Location_AddressLine");
			locationBuilder.Property(l => l.Country)
				.HasColumnName("Location_Country");
			locationBuilder.Property(l => l.StateOrRegion)
				.HasColumnName("Location_StateOrRegion");
			locationBuilder.Property(l => l.City)
				.HasColumnName("Location_City");
			
			locationBuilder.Property(l => l.Geometry)
				.HasColumnName("Location_Geometry")
				.HasColumnType("geography");
		});
		
		List<IdentityRole> roles =
		[
			new IdentityRole
			{
				Name = "Admin",
				NormalizedName = "ADMIN"
			},

			new IdentityRole
			{
				Name = "User",
				NormalizedName = "USER"
			}

		];
		builder.Entity<IdentityRole>().HasData(roles);
		
	}
}

