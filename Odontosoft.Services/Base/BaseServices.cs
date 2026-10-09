using Microsoft.Extensions.Logging;
using OdontoSoft.Data.Authentication;
using OdontoSoft.Data.Repository.UOW;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Odontosoft.Services
{
    public abstract class BaseServices
    {
        private readonly ILogger _logger;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAuthenticatedUser _authenticatedUser;

        protected BaseServices(ILogger logger, IUnitOfWork unitOfWork, IAuthenticatedUser authenticatedUser)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
            _authenticatedUser = authenticatedUser;
        }
    }
}
