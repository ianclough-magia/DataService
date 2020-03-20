using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using Connector.Dao;
using DataService.Model;
using Microsoft.AspNetCore.Mvc;

namespace DataService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class DataController : ControllerBase
    {
     private IDataDao _dataDao;

     public DataController(IDataDao dataDao)
     {
      _dataDao = dataDao;
     }
        
     /*
      * Save the form data at the end of an interview request
      * Request
      *     POST
      *     /formdata/<form-id>?userid=<user-id>
      *     {"form_data_json":"<form-data-json>"}
      * Response
      *     200
      *     {"form_id":<form-id>", "request_id":"<request-id>"}
      */
        [HttpPost("/formdata/{form_id}")]
        public Form SaveForm(string form_id, string user_id, Form form)
        { 
         Console.WriteLine("DataController.SaveForm");
         _dataDao.Save(user_id,form_id, form.form_data_json);
         string requestId = "0";
         return new Form {form_id = form_id, request_id = requestId};
        }

        /* 
         * Retrieve the form data for a specific request
         * Request
         *     GET
         *     /formdata/<form-id>/<request-id>
         * Response
         *     200
         *     {"form_id":"<form-id>", "request_id":"<request-id>", "form_status":"<form-status>", "form_data_json":"<form-data-json>"}
         */ 
        [HttpGet("/formdata/{form_id}/{request_id}")]
        public Form GetForm(string form_id, string request_id, string user_id)
        {
         Console.WriteLine("DataController.GetForm form_id=" + form_id + " request_id=" + request_id + " user_id=" + user_id);
         string jsonData = _dataDao.Load(user_id, form_id);
         return new Form{form_id = form_id, request_id = request_id, form_data_json = jsonData};
        }

        /* 
         * Save the form data at the end of an interview approval
         * Request
         *     PUT
         *     /formdata/<form-id>/<request-id>?userid=<user-id>
         *     {"form_status":"<form-status>", "form_data_json":"<form-data-json>"}
         * Response
         *     200
         */ 
        [HttpPost("/formdata/{form_id}/{request_id}")]
        public HttpResponseMessage SaveForm(string form_id, string request_id, string user_id, Form form)
        {
         Console.WriteLine("DataController.SaveForm form_id=" + form_id + " request_id=" + request_id + " user_id=" + user_id);
         _dataDao.Save(user_id, form_id, form.form_data_json);
         return new HttpResponseMessage(HttpStatusCode.OK);
        }
        /* 
         * Remove the form data
         * Request
         *     DELETE
         *     /formdata/<form-id>/<request-id>?userid=<user-id>
         * Response
         *     200
         */ 
        [HttpDelete("/formdata/{form_id}/{request_id}")]
        public HttpResponseMessage DeleteForm(string form_id, string request_id, string user_id)
        {
         Console.WriteLine("DataController.DeleteForm form_id=" + form_id + " request_id=" + request_id + " user_id=" + user_id);
         return new HttpResponseMessage(HttpStatusCode.OK);
        }
        /* 
         * Retrieve all form details with the specified status
         * Request
         *     GET
         *     /formdata?status=<status>
         * Response
         *     200
         *     [{"form_id":<form-id>", "request_id":"<request-id>"}]
         */
        [HttpGet("/formdata")]
        public List<Form> GetForm(string status)
        {
         Console.WriteLine("DataController.GetForm status=" + status);
         return new List<Form> {new Form {form_status = status} };
        }

        
    }
}