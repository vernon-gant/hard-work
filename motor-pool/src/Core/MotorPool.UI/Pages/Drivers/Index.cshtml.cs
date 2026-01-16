using AutoMapper;
using MotorPool.Domain;
using MotorPool.Persistence;
using MotorPool.Repository.Driver;
using MotorPool.Services.Drivers.Models;
using MotorPool.Services.Manager;
using MotorPool.UI.Pages.Shared;

namespace MotorPool.UI.Pages.Drivers;

public class IndexModel(DriverQueryRepository driverQueryRepository, IMapper mapper) : PagedModel
{

    public IList<DriverViewModel> Drivers { get; set; } = default!;

    public async Task OnGetAsync(int? currentPage)
    {
        CurrentPage = currentPage ?? 1;

        PagedResult<Driver> pagedResult = await driverQueryRepository.GetAllAsync(User.GetManagerId(), new ()
        {
            ElementsPerPage = ELEMENTS_PER_PAGE,
            CurrentPage = CurrentPage
        });

        TotalPages = pagedResult.TotalPages;
        Drivers = mapper.Map<List<DriverViewModel>>(pagedResult.Elements);
    }

}