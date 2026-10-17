using Microsoft.Extensions.Logging;
using OdontoSoft.Application.Abstractions;
using OdontoSoft.Application.Abstractions.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OdontoSoft.Application.Services.Base
{
  public abstract class BaseServices
  {
    protected readonly ILogger _logger;
    protected readonly IUnitOfWork _unitOfWork;
    protected readonly ICurrentUser _authenticatedUser;

    protected BaseServices(ILogger logger, IUnitOfWork unitOfWork, ICurrentUser authenticatedUser)
    {
      _logger = logger;
      _unitOfWork = unitOfWork;
      _authenticatedUser = authenticatedUser;
    }
  }
}
