using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace BankSystem.Models
{
    internal class ChangesClientArgs
    {
        public ChangesClientArgs(TypeChanges typeChanges, string text = "Без комментариев")
        {
            Text = text;
            MyTypeChanges = typeChanges;
            DateCreate = DateTime.Now;
            Str = $"{DateCreate} {MyTypeChanges} {Text}";
        }

        public enum TypeChanges
        {
            Create,
            Edit,
            Delete,
            OpenAccount,
            CloseAccount,
            Transfer,
            Refill
        }

        public TypeChanges MyTypeChanges { get; private set; }
        
        public DateTime DateCreate { get; }
        public string Text { get; private set; }

        public string Str { get; private set; }

        public override string ToString()
        {
            return $"{DateCreate} {MyTypeChanges} {Text}";
        }
    }
}
