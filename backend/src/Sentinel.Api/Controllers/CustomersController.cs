// ---Made By Destiny7 Softwares---
using Microsoft.AspNetCore.Mvc;
using Sentinel.Application;

namespace Sentinel.Api.Controllers;

[ApiController]
[Route("api/customers")]
public sealed class CustomersController(ISentinelReadService service) : ControllerBase
{
    [HttpGet]
    public ActionResult<IReadOnlyList<CustomerListItemDto>> GetAll() => Ok(service.GetCustomers());

    [HttpGet("{id:guid}")]
    public ActionResult<CustomerDetailDto> GetById(Guid id)
    {
        var customer = service.GetCustomer(id);
        return customer is null ? NotFound() : Ok(customer);
    }
}
