﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Inventory_Management_iTransition.Context;
using Inventory_Management_iTransition.Models;

namespace Inventory_Management_iTransition.Services
{
	public class IdFormatService
	{
        private static readonly Random _random = new Random();
        public static string GeneratePreview(List<CustomIdElement> elements)
        {
            if (elements == null || !elements.Any())
            {
                return "[No format defined]";
            }

            var preview = new StringBuilder();
            foreach (var element in elements.OrderBy(e => e.Order))
            {
                switch (element.Type)
                {
                    case IdElementType.FixedText:
                        preview.Append(element.ValueOrFormat ?? "");
                        break;
                    case IdElementType.Random20Bit:
                        preview.Append("A7B3C"); 
                        break;
                    case IdElementType.Random32Bit:
                        preview.Append("X9E4Z1A2"); 
                        break;
                    case IdElementType.Random6Digit:
                        preview.Append("123456");
                        break;
                    case IdElementType.Random9Digit:
                        preview.Append("987654321");
                        break;
                    case IdElementType.Guid:
                        preview.Append("...guid...");
                        break;
                    case IdElementType.DateTime:
                        try
                        {
                            preview.Append(DateTime.UtcNow.ToString(element.ValueOrFormat ?? "yyyyMMdd"));
                        }
                        catch (FormatException)
                        {
                            preview.Append("[invalid_date_format]");
                        }
                        break;
                    case IdElementType.Sequence:
                        preview.Append("001"); 
                        break;
                }
            }
            return preview.ToString();
        }

        public async Task<string> GenerateIdAsync(Inventory inventory, InMContext context)
        {
            var idBuilder = new StringBuilder();
            var elements = inventory.CustomIdElements.OrderBy(e => e.Order);

            foreach (var element in elements)
            {
                switch (element.Type)
                {
                    case IdElementType.FixedText: 
                        idBuilder.Append(element.ValueOrFormat);
                        break;
                    case IdElementType.Random20Bit: 
                        idBuilder.Append(GenerateRandomHex(5));
                        break;
                    case IdElementType.Random32Bit: 
                        idBuilder.Append(GenerateRandomHex(8));
                        break;
                    case IdElementType.Random6Digit: 
                        idBuilder.Append(_random.Next(0, 1000000).ToString("D6"));
                        break;
                    case IdElementType.Random9Digit: 
                        long random9Digit = (long)(_random.NextDouble() * 900000000L) + 100000000L;
                        idBuilder.Append(random9Digit);
                        break;
                    case IdElementType.Guid: 
                        idBuilder.Append(Guid.NewGuid().ToString());
                        break;
                    case IdElementType.DateTime: 
                        idBuilder.Append(DateTime.UtcNow.ToString(element.ValueOrFormat));
                        break;
                    case IdElementType.Sequence: 
                        var maxSequence = await context.Items
                            .Where(i => i.InventoryId == inventory.Id)
                            .Select(i => (int?)i.SequenceNumber)
                            .DefaultIfEmpty(0)
                            .MaxAsync();

                        var nextSequence = (maxSequence ?? 0) + 1;

                        idBuilder.Append(nextSequence.ToString(element.ValueOrFormat));
                        break;
                }
            }

            return idBuilder.ToString();
        }

        private string GenerateRandomHex(int length)
        {
            byte[] buffer = new byte[length / 2 + 1];
            _random.NextBytes(buffer);
            string hex = string.Concat(buffer.Select(x => x.ToString("X2")).ToArray());
            return hex.Substring(0, length);
        }

    }
}