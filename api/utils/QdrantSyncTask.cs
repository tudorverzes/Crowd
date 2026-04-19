﻿using api.data;
using api.service;
using Microsoft.EntityFrameworkCore;
using Qdrant.Client;
using Qdrant.Client.Grpc;

namespace api.utils;

public static class QdrantSyncTask
{
    public static async Task SyncMissingEventsAsync(IServiceProvider serviceProvider)
    {
        try
        {
            using var scope = serviceProvider.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var aiService = scope.ServiceProvider.GetRequiredService<IAiService>();
            var qdrantService = scope.ServiceProvider.GetRequiredService<IQdrantService>();
            var qdrantClient = scope.ServiceProvider.GetRequiredService<QdrantClient>();

            Console.WriteLine("[SYNC] Verifying missing events in Qdrant...");
            
            var allEvents = await dbContext.Events.Include(e => e.Location).ToListAsync();
            
            if (allEvents.Count == 0)
            {
                Console.WriteLine("[SYNC] No events exist in the SQL database.");
                return;
            }

            var allSqlIds = allEvents.Select(e => (ulong)e.Id).ToList();
            var existingInQdrantIds = new HashSet<ulong>();
            
            for (int i = 0; i < allSqlIds.Count; i += 1000)
            {
                var batchIds = allSqlIds.Skip(i).Take(1000).Select(id => (PointId)id).ToList();
                
                var existingPoints = await qdrantClient.RetrieveAsync(
                    collectionName: "Events", 
                    ids: batchIds, 
                    withVectors: false, 
                    withPayload: false);
                    
                foreach (var point in existingPoints)
                {
                    existingInQdrantIds.Add(point.Id.Num);
                }
            }

            var missingEvents = allEvents.Where(e => !existingInQdrantIds.Contains((ulong)e.Id)).ToList();

            if (missingEvents.Count == 0)
            {
                Console.WriteLine("[SYNC] Perfect! All events are already synchronized in Qdrant.");
                return;
            }

            Console.WriteLine($"[SYNC] Found {missingEvents.Count} missing events in Qdrant. Starting vector generation...");

            var count = 0;
            foreach (var ev in missingEvents)
            {
                var textForAi = $"{ev.Name}. {ev.Description}";
                
                var vector = aiService.GenerateVector(textForAi);
                
                await qdrantService.UpsertEventAsync(ev, vector);
                
                count++;
                if (count % 10 == 0 || count == missingEvents.Count) // Display progress every 10 events
                {
                    Console.WriteLine($"[SYNC] Progress: {count} / {missingEvents.Count} events processed.");
                }
            }

            Console.WriteLine("[SYNC] Synchronization completed successfully!");
        }
        catch (Exception ex)
        {
            // Log the error but don't fail - the application will continue
            Console.WriteLine($"[SYNC] Warning: Qdrant synchronization failed: {ex.Message}. The application will continue with limited functionality.");
        }
    }
}