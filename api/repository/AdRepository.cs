﻿using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.data;
using api.model;
using Microsoft.EntityFrameworkCore;

namespace api.repository;

public class AdRepository : IAdRepository
{
	private readonly ApplicationDbContext _context;

	public AdRepository(ApplicationDbContext context)
	{
		_context = context;
	}

	public async Task<List<Ad>> GetAllAsync()
	{
		return await _context.Ads
			.Include(a => a.Targets)
			.ThenInclude(t => ((GeoRadiusTarget)t).Location)
			.ToListAsync();
	}

	public async Task<List<Ad>> GetAllForUserAsync(string userId)
	{
		return await _context.Ads
			.Where(ad => ad.OwnerId == userId)
			.Include(a => a.Targets)
			.ThenInclude(t => ((GeoRadiusTarget)t).Location)
			.ToListAsync();
	}

	public async Task<List<Ad>> GetAllUnapprovedAsync()
	{
		return await _context.Ads
			.Where(ad => !ad.IsApproved)
			.Include(a => a.Targets)
			.ThenInclude(t => ((GeoRadiusTarget)t).Location)
			.ToListAsync();
	}

	public async Task<Ad?> GetByIdAsync(string id)
	{
		return await _context.Ads
			.Include(a => a.Targets)
			.ThenInclude(t => ((GeoRadiusTarget)t).Location)
			.FirstOrDefaultAsync(ad => ad.Id == id);
	}

	public async Task<Ad?> CreateAsync(Ad ad)
	{
		await _context.Ads.AddAsync(ad);
		await _context.SaveChangesAsync();
		return ad;
	}

	public async Task<Ad?> UpdateAsync(Ad ad)
	{
		throw new System.NotImplementedException();
	}

	public async Task<Ad?> DeleteAsync(string id)
	{
		var ad = await _context.Ads.FirstOrDefaultAsync(a => a.Id == id);
		if (ad == null)
		{
			return null;
		}
		
		_context.Ads.Remove(ad);
		await _context.SaveChangesAsync();
		return ad;
	}

	public async Task<Ad?> ChangeApprovalStatusAsync(string id, bool isApproved)
	{
		var ad = await _context.Ads.FirstOrDefaultAsync(a => a.Id == id);
		if (ad == null)
		{
			return null;
		}
		
		ad.IsApproved = isApproved;
		await _context.SaveChangesAsync();
		return ad;
	}

	public async Task<Ad?> ChangeAdStatusAsync(string id, AdStatus newStatus)
	{
		var ad = await _context.Ads.FirstOrDefaultAsync(a => a.Id == id);
		if (ad == null)
		{
			return null;
		}
		
		ad.Status = newStatus;
		await _context.SaveChangesAsync();
		return ad;
	}
}