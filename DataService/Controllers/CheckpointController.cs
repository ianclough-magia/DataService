using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Net.Http;
using Connector.Dao;
using DataService.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Server.Kestrel.Core;

namespace DataService.Controllers
{
    [ApiController]
    [Route("checkpoint")]
    public class CheckpointController : ControllerBase
    {
        private ICheckpointDao _checkpointDao;

        public CheckpointController(ICheckpointDao checkpointDao)
        {
            _checkpointDao = checkpointDao;
        }
        
        /*
         * Create a checkpoint for a specific user/form, called from OPA on screen transitions (if configured for form)
         * Request
         *    POST
         *    /checkpoint/<form-id>?userid=<user-id>
         *    {"checkpoint_data","<checkpoint-data>"}
         *Response
         *    200
         */
        [HttpPost("/checkpoint/{form_id}")]
        public Checkpoint SetCheckpoint(string form_id, string user_id)
        {
            Console.WriteLine($"CheckpointController.SetCheckpoint form_id={form_id} user_id={user_id}");
            return new Checkpoint {checkpoint_id = "0"};
        }
        /*
         *Retrieve a checkpoint for a specific user/form, called from OPA to resume a previously saved checkpoint for that user
         *Request
         *    GET
         *    /checkpoint/<form-id>?userid=<user-id>
         *Response
         *    200
         *    {"form_id":"<form-id>", "checkpoint_id":"<checkpoint-id>", "user_id":"<user-id>", "checkpoint_data","<checkpoint-data>"}
         *    404
         */
        [HttpGet("/checkpoint/{form_id}")]
        public Checkpoint GetCheckpoint([Required]string form_id, [Required]string user_id)
        {
            Console.WriteLine($"CheckpointController.GetCheckpoint form_id={form_id} user_id={user_id}");
            Checkpoint checkpoint = _checkpointDao.GetCheckpointData(user_id, form_id);
            return checkpoint;
        }
        /*
         *Delete a checkpoint for specific user/form 
         *Request
         *    DELETE
         *    /checkpoint/<form-id>?userid=<user-id>
         *Response
         *    200
         */
        [HttpDelete("/checkpoint/{form_id}")]
        public Checkpoint DeleteCheckpoint(string form_id, string user_id)
        {
            return new Checkpoint{form_id = form_id};
        }
        /*
         *List the forms that are in-flight for a specific user
         *Request
         *    GET
         *    /checkpoint?userid=<user-id>
         *Response
         *    200
         *    [{"form_id":"<form-id>", "checkpoint_id":"<checkpoint-id>", "user_id":"<user-id>"}]
         */
        [HttpGet]
        public IEnumerable<Checkpoint> Get(string user_id)
        {
            return new List<Checkpoint>() {new Checkpoint{user_id = user_id}};
        }

    }
}