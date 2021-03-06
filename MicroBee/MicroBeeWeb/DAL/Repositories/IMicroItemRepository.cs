﻿using System.Linq;
using System.Threading.Tasks;
using MicroBee.Web.DAL.Entities;

namespace MicroBee.Web.DAL.Repositories
{
    public interface IMicroItemRepository
    {
		Task<MicroItem> FindAsync(int id);
		IQueryable<MicroItem> GetAll();
		Task<MicroItem> AddAsync(MicroItem item);
		Task<MicroItem> UpdateAsync(MicroItem item);
		Task DeleteAsync(int id);
	}
}
