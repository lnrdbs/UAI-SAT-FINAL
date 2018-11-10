using System;
using System.Collections.Generic;
using System.Text;

namespace RealtimeSample.Firebase
{
   public class ChatMessage
    {
        public ChatMessage()
        {
            Time = DateTime.Now;
        }
        public string Message { get; set; }
        public string Nikname { get; set; }
        public DateTime Time { get; set; }
    }

  
}
