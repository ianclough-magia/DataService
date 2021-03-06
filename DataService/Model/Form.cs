﻿using System;
using System.ComponentModel.Design;

namespace DataService.Model
{
    public class Form
    {
        public string form_id { get; set; }
        public string request_id { get; set; }
        
        public string form_stage { get; set; }
        
        public string user_id { get; set; }
        
        public DateTime created { get; set; }
        
        public DateTime updated { get; set; }
        public string form_data_json { get; set; }
    }
}