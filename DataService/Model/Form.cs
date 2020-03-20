using System.ComponentModel.Design;

namespace DataService.Model
{
    public class Form
    {
        public string form_id { get; set; }
        public string request_id { get; set; }
        public string form_status { get; set; }
        public string form_data_json { get; set; }
    }
}