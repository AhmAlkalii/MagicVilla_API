using MagicVilla_VillaApi.Data;
using MagicVilla_VillaApi.Logging;
using MagicVilla_VillaApi.Models;
using MagicVilla_VillaApi.Models.Dto;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using System.Runtime.InteropServices;

namespace MagicVilla_VillaApi.Controllers
{
    [Route("api/VillaAPI")]  //We add a route name here, it could also be [Route("api/[controller]")]
    [ApiController]  //this helps us with validation as well it does it by defualt
    public class VillaApiController : ControllerBase  //Rember to import controllerbase
    {

        //In order to implement logger 

        //private readonly ILogger<VillaApiController> _logger;

        //public VillaApiController(ILogger<VillaApiController> logger) 
        //{
        //    _logger = logger;
        //}


        //using our own logger interface and class
        private readonly ILogging _logger;

        public VillaApiController(ILogging logger) 
        { 
           _logger = logger;
        }


        [HttpGet] //Tell us the type of request
        /*To be able add status codes we add the Actionresult and wrap it around the function name 
         * then in the return statement we add Ok(what want to return)
         * just like in express where we say res.status(200).json('something')
        */
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult<IEnumerable<VillaDto>> GetVillas()  //Villa here is the model name
        {
            //return new List<VillaDto>
            //{
            //    new VillaDto{Id= 1, Name = "Pool View"},
            //    new VillaDto{Id =2, Name= "Beach View"}
            //};

            //we store the data inside the VillaStore class now we are calling it  instead of what we had before
            //return VillaStore.villaList;


            //_logger.LogInformation("Getting All Villas");  this is for third party logger

            //this is for our own logger
            _logger.Log("Getting All Villas", "");
            return Ok(VillaStore.villaList);


        }

        //here we added the id to overload the exisitng method so we are getting a villa by id
        //we can be explicts by specify what type the id is [HttpGet("{id: int }")]
        //to give the httpget an explicit name we add [HttpGet("id", Name="GetVilla")]
        [HttpGet("id", Name = "GetVilla")]


        //There is something called documenting in .net where we declare what typeof respones the function will return
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]

        //we remove the Ienumerable because we are only expecting it to return one thing 
        public ActionResult<VillaDto> GetVilla(int id)  
        {
            //lets add validation
            if(id == 0)
            {
                //_logger.LogError("Get Villa Error with Id" + id);
                _logger.Log("Get Villa Error with Id" + id, "error");
                return BadRequest();
            }
            var villa = VillaStore.villaList.FirstOrDefault(u => u.Id == id);
            //the u => u.Id == id checks if the parameter passed is matches an id in our store so acts like a for loop
            //return VillaStore.villaList.FirstOrDefault(u => u.Id == id);
            if(villa == null)
            {
                return NotFound();
            }
            return Ok(villa);

            //in .net we use the actual name of the status codes and not the codes like Ok instead of 200 
        }


        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        //here we say [frombody] its exactly ike req.body
        public ActionResult<VillaDto> CreateVilla([FromBody]VillaDto villaDto)
        {
            //Adding custom validation and checking if villa exist already
            if(VillaStore.villaList.FirstOrDefault(u => u.Name.ToLower() == villaDto.Name.ToLower()) != null)
            {
                ModelState.AddModelError("Custom Error", "Villa already Exists!");
                return BadRequest(ModelState);
            }


            if(villaDto == null)
            {
                return BadRequest(villaDto);
            }

            if(villaDto.Id > 0)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
            villaDto.Id = VillaStore.villaList.OrderByDescending(u => u.Id).FirstOrDefault().Id + 1;
            VillaStore.villaList.Add(villaDto);

            //return Ok(villaDto);
            return CreatedAtRoute("GetVilla", new { id = villaDto.Id }, villaDto);

        }


        [HttpDelete("id", Name="DeleteVilla")]

        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        //With IActionResult it returns Nocontent which perfect for delete request unlike just Action Result which will return other status codes
        public IActionResult DeleteVilla(int id)
        {
            if (id == 0)
            {
                return BadRequest();
            }
            var villa = VillaStore.villaList.FirstOrDefault(u => u.Id == id);
            //the u => u.Id == id checks if the parameter passed is matches an id in our store so acts like a for loop
            //return VillaStore.villaList.FirstOrDefault(u => u.Id == id);
            if (villa == null)
            {
                return NotFound();
            }
            VillaStore.villaList.Remove(villa);
            return NoContent();
        }


        [HttpPut("{id:int}", Name = "UpdateVilla")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult UpdateVilla(int id, [FromBody]VillaDto villaDto)
        {
            if(villaDto == null || id != villaDto.Id)
            {
                return BadRequest();
            }
            var villa = VillaStore.villaList.FirstOrDefault(u => u.Id == id);

            villa.Name = villaDto.Name;
            villa.Sqft = villaDto.Sqft;
            villa.Occupancy = villaDto.Occupancy;

            return NoContent();
        }


        [HttpPatch("{id:int}", Name = "UpdatePartialVilla")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]

        public IActionResult UpdatePartialVilla(int id, JsonPatchDocument<VillaDto> patchDto)
        {
            if (patchDto == null || id == 0)
            {
                return BadRequest();
            }
            var villa = VillaStore.villaList.FirstOrDefault(u => u.Id == id);
            if(villa == null)
            {
                return BadRequest();
            }
            patchDto.ApplyTo(villa, ModelState);
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            return NoContent();
        }
        
    }
}
