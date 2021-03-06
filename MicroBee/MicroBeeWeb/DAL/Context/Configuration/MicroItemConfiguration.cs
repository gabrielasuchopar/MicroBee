﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MicroBee.Web.DAL.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MicroBee.Web.DAL.Context.Configuration
{
	class MicroItemConfiguration : IEntityTypeConfiguration<MicroItem>
	{
		public void Configure(EntityTypeBuilder<MicroItem> builder)
		{
			builder.HasKey(m => m.Id);
			builder.HasOne(m => m.Category)
				.WithMany();

			//created items of the user
			builder.HasOne<ApplicationUser>()
				.WithMany(ap => ap.CreatedItems)
				.HasPrincipalKey(ap => ap.UserName)
				.HasForeignKey(m => m.OwnerName);

			//accepted items of the user
			builder.HasOne<ApplicationUser>()
				.WithMany(ap => ap.AcceptedItems)
				.HasPrincipalKey(ap => ap.UserName)
				.HasForeignKey(m => m.WorkerName);

			builder.Property(m => m.Price).HasColumnType("Money");

		}
	}
}
