using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using Inventory_Management_iTransition.Models;

namespace Inventory_Management_iTransition.Services
{
	public class IdFormatService
	{
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

        // We can add the actual ID generation logic here later
        // public static string GenerateNewId(List<CustomIdElement> elements, int lastSequence) { ... }
    }
}