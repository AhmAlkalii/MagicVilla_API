using MagicVilla_VillaApi.Data;
using MagicVilla_VillaApi.Models;
using MagicVilla_VillaApi.Models.Dto;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace MagicVilla_VillaApi.Controllers
{
    [Route("api/VillaAPI")]  //We add a route name here, it could also be [Route("api/[controller]")]
    [ApiController]
    public class VillaApiController : ControllerBase  //Rember to import controllerbase
    {
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
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        //here we say [frombody] its exactly ike req.body
        public ActionResult<VillaDto> CreateVilla([FromBody]VillaDto villaDto)
        {
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
    }
}
