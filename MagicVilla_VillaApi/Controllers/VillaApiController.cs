using MagicVilla_VillaApi.Data;
using MagicVilla_VillaApi.Logging;
using MagicVilla_VillaApi.Models;
using MagicVilla_VillaApi.Models.Dto;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics.CodeAnalysis;
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
        //private readonly ILogging _logger;

        //public VillaApiController(ILogging logger) 
        //{ 
        //   _logger = logger;
        //}


        //Now we are moving away from using the store and using the db instead
        private readonly ApplicationDbContext _db;

        public VillaApiController(ApplicationDbContext db)
        {
            _db = db;
        }




        [HttpGet] //Tell us the type of request
        /*To be able add status codes we add the Actionresult and wrap it around the function name 
         * then in the return statement we add Ok(what want to return)
         * just like in express where we say res.status(200).json('something')
        */
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult<IEnumerable<VillaDto>> GetVillas()  //Villa here is the model name
        {

            //All these used the store not db
            //return new List<VillaDto>
            //{
            //    new VillaDto{Id= 1, Name = "Pool View"},
            //    new VillaDto{Id =2, Name= "Beach View"}
            //};

            //we store the data inside the VillaStore class now we are calling it  instead of what we had before
            //return VillaStore.villaList;


            //_logger.LogInformation("Getting All Villas");  this is for third party logger

            //this is for our own logger
            //_logger.Log("Getting All Villas", "");

            //return Ok(VillaStore.villaList);

            //Now we retrieve all the villas from the db instead of the store we access the db field and the name of the table

            return Ok(_db.Villas);

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
                //_logger.Log("Get Villa Error with Id" + id, "error");
                return BadRequest();
            }
            //Also used store not db
            //var villa = VillaStore.villaList.FirstOrDefault(u => u.Id == id);
            //the u => u.Id == id checks if the parameter passed is matches an id in our store so acts like a for loop
            //return VillaStore.villaList.FirstOrDefault(u => u.Id == id);


            //This uses db 
            var villa = _db.Villas.FirstOrDefault(u => u.Id == id);

            if (villa == null)
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
            if(_db.Villas.FirstOrDefault(u => u.Name.ToLower() == villaDto.Name.ToLower()) != null)
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
            //we dont need to assign id because with db it is done for us
            //villaDto.Id = VillaStore.villaList.OrderByDescending(u => u.Id).FirstOrDefault().Id + 1;

            //Adding a record to our store 
            //VillaStore.villaList.Add(villaDto);

            /*Here we add a record to our db instead
             we cannot us villDto becuase it has type VillDto so instead we will give an instance of 
            our schema like below and pass that instead
             */
            Villa model = new()
            {
                Amenity = villaDto.Amenity,
                Details = villaDto.Details,
                ImageUrl = villaDto.ImageUrl,
                Name = villaDto.Name,
                Occupancy = villaDto.Occupancy,
                Rate = villaDto.Rate,
                Sqft = villaDto.Sqft
            };


            _db.Villas.Add(model);
            //now save the changes
            _db.SaveChanges();

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
            //All these used the store
            //var villa = VillaStore.villaList.FirstOrDefault(u => u.Id == id);
            //the u => u.Id == id checks if the parameter passed is matches an id in our store so acts like a for loop
            //return VillaStore.villaList.FirstOrDefault(u => u.Id == id);

            //Use db instead
            var villa = _db.Villas.FirstOrDefault(u => u.Id == id);


            if (villa == null)
            {
                return NotFound();
            }
            //removing with store
            //VillaStore.villaList.Remove(villa);

            //removing with db 
            _db.Villas.Remove(villa);
            //save
            _db.SaveChanges();
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
            //now longer need these for store
            //var villa = VillaStore.villaList.FirstOrDefault(u => u.Id == id);
            //villa.Name = villaDto.Name;
            //villa.Sqft = villaDto.Sqft;
            //villa.Occupancy = villaDto.Occupancy;

            //easier to update with db
            Villa model = new()
            {
                Amenity = villaDto.Amenity,
                Details = villaDto.Details,
                ImageUrl = villaDto.ImageUrl,
                Name = villaDto.Name,
                Occupancy = villaDto.Occupancy,
                Rate = villaDto.Rate,
                Sqft = villaDto.Sqft
            };

            _db.Villas.Update(model);
            _db.SaveChanges();
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
            //use db not store
            //var villa = VillaStore.villaList.FirstOrDefault(u => u.Id == id);

            var villa = _db.Villas.AsNoTracking().FirstOrDefault(u => u.Id == id);
            VillaDto villaDto = new()
            {
                Amenity = villa.Amenity,
                Details = villa.Details,
                ImageUrl = villa.ImageUrl,
                Name = villa.Name,
                Occupancy = villa.Occupancy,
                Rate = villa.Rate,
                Sqft = villa.Sqft
            };

            if (villa == null)
            {
                return BadRequest();
            }
            //patchDto.ApplyTo(villa, ModelState);
            patchDto.ApplyTo(villaDto, ModelState);

            Villa model = new()
            {
                Amenity = villaDto.Amenity,
                Details = villaDto.Details,
                ImageUrl = villaDto.ImageUrl,
                Name = villaDto.Name,
                Occupancy = villaDto.Occupancy,
                Rate = villaDto.Rate,
                Sqft = villaDto.Sqft
            };

            _db.Villas.Update(model);
            _db.SaveChanges();
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            return NoContent();
        }
        
    }
}
