using OdontoSoft.Application.Abstractions.Persistence;

namespace OdontoSoft.Infrastructure.Data.Repository.UOW
{
  public partial class UnitOfWork
  {
    public IEstadoTurnoRepository EstadoTurnoRepository { get; private set; }
    public IFichaTratamientoRepository FichaTratamientoRepository { get; private set; }
    public IHistoriaClinicaRepository HistoriaClinicaRepository { get; private set; }
    public IImagenPacienteRepository ImagenPacienteRepository { get; private set; }
    public ILocalidadRepository LocalidadRepository { get; private set; }
    public IOdontogramaPiezaRepository OdontogramaPiezaRepository { get; private set; }
    public IPacienteRepository PacienteRepository { get; private set; }
    public IPagoRepository PagoRepository { get; private set; }
    public IPeriodontogramaRepository PeriodontogramaRepository { get; private set; }
    public IPresupuestoRepository PresupuestoRepository { get; private set; }
    public IPresupuestoDetalleRepository PresupuestoDetalleRepository { get; private set; }
    public IProfesionalRepository ProfesionalRepository { get; private set; }
    public IProvinciaRepository ProvinciaRepository { get; private set; }
    public ITratamientoRepository TratamientoRepository { get; private set; }
    public ITratamientoPrecioRepository TratamientoPrecioRepository { get; private set; }
    public ITurnoRepository TurnoRepository { get; private set; }
    public IUbicacionRepository UbicacionRepository { get; private set; }

    private void InstanceRepos()
    {
      EstadoTurnoRepository = new EstadoTurnoRepository(_context, _logger, _authenticatedUser);
      FichaTratamientoRepository = new FichaTratamientoRepository(_context, _logger, _authenticatedUser);
      HistoriaClinicaRepository = new HistoriaClinicaRepository(_context, _logger, _authenticatedUser);
      ImagenPacienteRepository = new ImagenPacienteRepository(_context, _logger, _authenticatedUser);
      LocalidadRepository = new LocalidadRepository(_context, _logger, _authenticatedUser);
      OdontogramaPiezaRepository = new OdontogramaPiezaRepository(_context, _logger, _authenticatedUser);
      PacienteRepository = new PacienteRepository(_context, _logger, _authenticatedUser);
      PagoRepository = new PagoRepository(_context, _logger, _authenticatedUser);
      PeriodontogramaRepository = new PeriodontogramaRepository(_context, _logger, _authenticatedUser);
      PresupuestoRepository = new PresupuestoRepository(_context, _logger, _authenticatedUser);
      PresupuestoDetalleRepository = new PresupuestoDetalleRepository(_context, _logger, _authenticatedUser);
      ProfesionalRepository = new ProfesionalRepository(_context, _logger, _authenticatedUser);
      ProvinciaRepository = new ProvinciaRepository(_context, _logger, _authenticatedUser);
      TratamientoRepository = new TratamientoRepository(_context, _logger, _authenticatedUser);
      TratamientoPrecioRepository = new TratamientoPrecioRepository(_context, _logger, _authenticatedUser);
      TurnoRepository = new TurnoRepository(_context, _logger, _authenticatedUser);
      UbicacionRepository = new UbicacionRepository(_context, _logger, _authenticatedUser);
    }
  }
}
