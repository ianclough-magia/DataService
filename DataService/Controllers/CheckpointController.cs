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
        public Checkpoint SetCheckpoint([Required]string form_id, [Required]string user_id, Checkpoint checkpoint)
        {
            Console.WriteLine($"CheckpointController.SetCheckpoint form_id={form_id} user_id={user_id}");
            _checkpointDao.SetCheckpointData(user_id, form_id, checkpoint.checkpoint_data);
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
        public IActionResult GetCheckpoint([Required]string form_id, [Required]string user_id)
        {
            Console.WriteLine($"CheckpointController.GetCheckpoint form_id={form_id} user_id={user_id}");
            Checkpoint checkpoint = _checkpointDao.GetCheckpointData(user_id, form_id);
            if (checkpoint == null)
            {
                return NotFound();
            }
            return Ok(checkpoint);
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
        public IActionResult DeleteCheckpoint([Required] string form_id, [Required] string user_id)
        {
            bool deleted = _checkpointDao.DeleteCheckpoint(user_id, form_id);
            if (deleted)
            {
                return Ok();
            }
            else
            {
                return NotFound();
            }
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
        public IEnumerable<Checkpoint> Get([Required] string user_id)
        {
            return _checkpointDao.ListCheckpoints(user_id);
        }

    }
}