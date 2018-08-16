﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;
using MicroBee.Web.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace MicroBee.Web.Context
{
	public class MicroBeeContext : IdentityDbContext
	{
		public MicroBeeContext(DbContextOptions<MicroBeeContext> options) : base(options)
		{
		}

		DbSet<MicroItem> MicroItems { get; set; }
		DbSet<ItemCategory> Categories { get; set; }

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.Entity<MicroItem>().HasKey(m => m.Id);
			modelBuilder.Entity<MicroItem>().HasOne(m => m.Category);

			modelBuilder.Entity<ItemCategory>().HasKey(c => c.Id);
		}
	}
}