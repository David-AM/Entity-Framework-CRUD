using EF_CRUD.Common;
using EF_CRUD.Dtos;
using EF_CRUD.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EF_CRUD.Controllers;

// Any signed-in user can read. Only Admin can create, update or delete.
[Authorize]
public sealed class ProductsController(IProductService service) : BaseApiController
{
    [HttpGet]
    public async Task<IActionResult> GetAll() => Respond(await service.GetAllAsync());

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id) => Respond(await service.GetByIdAsync(id));

    [HttpPost]
    [Authorize(Roles = Roles.Admin)]
    public async Task<IActionResult> Create(ProductRequestDto request) =>
        Respond(await service.CreateAsync(request), nameof(GetById), p => new { id = p.Id });

    [HttpPut("{id:int}")]
    [Authorize(Roles = Roles.Admin)]
    public async Task<IActionResult> Update(int id, ProductRequestDto request) =>
        Respond(await service.UpdateAsync(id, request));

    [HttpDelete("{id:int}")]
    [Authorize(Roles = Roles.Admin)]
    public async Task<IActionResult> Delete(int id) => RespondEmpty(await service.DeleteAsync(id));
}
