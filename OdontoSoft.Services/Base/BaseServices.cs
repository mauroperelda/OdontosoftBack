using Microsoft.Extensions.Logging;
using OdontoSoft.Infrastructure.Data.Authentication;
using OdontoSoft.Infrastructure.Data.Repository.UOW;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Odontosoft.Services
{
  public abstract class BaseServices
  {
    protected readonly ILogger _logger;
    protected readonly IUnitOfWork _unitOfWork;
    protected readonly IAuthenticatedUser _authenticatedUser;

    protected BaseServices(ILogger logger, IUnitOfWork unitOfWork, IAuthenticatedUser authenticatedUser)
    {
      _logger = logger;
      _unitOfWork = unitOfWork;
      _authenticatedUser = authenticatedUser;
    }
  }
}
