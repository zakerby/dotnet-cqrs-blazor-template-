using System.Threading;
using System.Threading.Tasks;
using Core.Backend.Application.Interfaces;
using Core.Backend.Application.Models;
using Core.Backend.Domain.Models;
using MediatR;
using FluentValidation;
using Serilog;

namespace Core.Backend.Application.Queries.Project
{
    public class GetProjectByIdQueryHandler : IRequestHandler<GetProjectByIdQuery, QueryResult<Domain.Models.Project>>
    {
        private readonly IValidator<GetProjectByIdQuery> _validator;
        private readonly ILogger _logger;

        public GetProjectByIdQueryHandler(
            ILogger logger,
            IValidator<GetProjectByIdQuery> validator)
        {
            _logger = logger;
            _validator = validator;
        } 

        public async Task<QueryResult<Domain.Models.Project>> Handle(GetProjectByIdQuery request, CancellationToken cancellationToken)
        {
            var validation = _validator.Validate(request);

            if (!validation.IsValid)
            {
                _logger.Error("Get project by id with id {Id} produced errors on validation {Errors}", request.Id, validation.ToString());
                return new QueryResult<Domain.Models.Project>(result: default(Domain.Models.Project), type: QueryResultTypeEnum.InvalidInput);
            }
            var project = new  Domain.Models.Project()
            {
                Id = request.Id,
                Name = "Test Project",
            };

            if (project == null)
            {
                return new QueryResult<Domain.Models.Project>(result: project, type: QueryResultTypeEnum.NotFound);
            }
            return new QueryResult<Domain.Models.Project>(result: project, type: QueryResultTypeEnum.Success);
        }

    }
}