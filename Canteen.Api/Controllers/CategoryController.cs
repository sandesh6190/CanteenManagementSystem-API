using Asp.Versioning;
using Canteen.Service.Interface;
using Canteen.Shared.Dtos.Category;
using Canteen.Shared.Dtos.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Canteen.Api.Controllers;

[Authorize]
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{v:apiVersion}/[controller]")]
[Produces("application/json")]
public class CategoryController(ICategoryService categoryService) : Controller
{
    #region PrivateMethods

    private static ResponseDto ValidateCategory(CategoryDto category)
    {
        if (string.IsNullOrWhiteSpace(category.CategoryName))
        {
            return new ResponseDto
            {
                IsSuccess = false,
                Message = "Category Name is required",
                StatusCode = (int)HttpStatusCode.BadRequest
            };
        }

        return new ResponseDto
        {
            IsSuccess = true,
            StatusCode = (int)HttpStatusCode.OK
        };
    }

    #endregion

    #region PublicMethods

    [HttpGet("getcategories")]
    [ProducesResponseType(typeof(ResponseDto), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
    public async Task<ActionResult> GetCategoriesAsync()
    {
        var response = await categoryService.GetCategoriesAsync()
            .ConfigureAwait(false);
        return Ok(response);
    }

    [HttpGet("getcategory")]
    [ProducesResponseType(typeof(ResponseDto),(int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
    public async Task<ActionResult> GetCategoryAsync(int id)
    {
        if(id <= 0)
        {
            return BadRequest(new ResponseDto
            {
                IsSuccess = false,
                Message = "Invalid Id",
                StatusCode = (int)HttpStatusCode.BadRequest
            });
        }
        
        var response = await categoryService.GetCategoryAsync(id)
            .ConfigureAwait(false);
        return response is null ? NoContent() : Ok(response);
    }

    [HttpPost("submitcategory")]
    [ProducesResponseType(typeof (ResponseDto),(int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
    public async Task<ActionResult> SubmitCategoryAsync([FromBody]CategoryDto categoryDto)
    {
        var validationResult = ValidateCategory(categoryDto);
        if (!validationResult.IsSuccess)
        {
            return BadRequest(validationResult);
        }
        
        var response = await categoryService.SubmitCategoryAsync(categoryDto) 
            .ConfigureAwait(false);
        return Ok(response);
    }

    [HttpDelete("deletecategory")]
    [ProducesResponseType(typeof(ResponseDto), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
    public async Task<ActionResult> DeleteCategoryAsync(int id)
    {
        var response =await categoryService.DeleteCategoryAsync(id) 
            .ConfigureAwait(false);
        return Ok(response);
    }

    #endregion
}