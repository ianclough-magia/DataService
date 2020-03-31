using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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
      *     /formdata/<form-id>
      *     {"form_id":"<form-id>","stage":"<stage>","user_id":"<user-id>","form_data_json":"<form-data-json>"}
      * Response
      *     200
      *     {"form_id":<form-id>","request_id":"<request-id>","stage":"<stage>","user_id":"<user-id>"}
      */
        [HttpPost("/formdata/{form_id}")]
        public Form SaveForm(string form_id, Form form)
        { 
         Console.WriteLine($"DataController.SaveForm form_id={form_id} form={form}");
         form.form_id = form_id;
         Form resultForm = _dataDao.Save(form);
         return new Form {form_id = form_id, request_id = resultForm.request_id};
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
        public Form GetForm(string form_id, string request_id)
        {
         Console.WriteLine($"DataController.GetForm form_id={form_id} request_id={request_id}");
         Form form = _dataDao.Load(form_id, request_id);
         return form;
        }

        /* 
         * Save the form data at the end of an interview approval
         * Request
      *     POST
      *     /formdata/<form-id>
      *     {"form_id":"<form-id>","stage":"<stage>","user_id":"<user-id>","form_data_json":"<form-data-json>"}
      * Response
      *     200
      *     {"form_id":<form-id>","request_id":"<request-id>","stage":"<stage>","user_id":"<user-id>"}
         */ 
        [HttpPost("/formdata/{form_id}/{request_id}")]
        public HttpResponseMessage SaveForm(string form_id, string request_id, Form form)
        {
         Console.WriteLine($"DataController.SaveForm form_id={form_id} request_id={request_id} form={form}");
         form.form_id = form_id;
         form.request_id = request_id;
         _dataDao.Save(form);
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
        public HttpResponseMessage DeleteForm([Required] string form_id, [Required] string request_id)
        {
         Console.WriteLine("DataController.DeleteForm form_id=" + form_id + " request_id=" + request_id);
         return new HttpResponseMessage(HttpStatusCode.OK);
        }
        /* 
         * Retrieve all form details with the specified status
         * Request
         *     GET
         *     /formdata?stage=<stage>
         * Response
         *     200
         *     [{"form_id":<form-id>", "request_id":"<request-id>"}]
         */
        [HttpGet("/formdata")]
        public List<Form> GetForm(string stage)
        {
         Console.WriteLine("DataController.GetForm status=" + stage);
         List<Form> forms = _dataDao.GetByStage(stage);
         return forms;
        }

        
    }
}