using MediatR;
using Yomimono.Application.Common;
using Yomimono.Application.Reports.DTOs;

namespace Yomimono.Application.Reports.Queries;

public record GetReportsQuery : IRequest<Result<ReportDto>>;
