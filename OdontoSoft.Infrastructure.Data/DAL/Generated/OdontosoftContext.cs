using OdontoSoft.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace OdontoSoft.Infrastructure.Data.DAL
{
  public partial class OdontosoftContext
  {
    public virtual DbSet<EstadoTurno> EstadoTurno { get; set; }
    public virtual DbSet<FichaTratamiento> FichaTratamiento { get; set; }
    public virtual DbSet<HistoriaClinica> HistoriaClinica { get; set; }
    public virtual DbSet<ImagenPaciente> ImagenPaciente { get; set; }
    public virtual DbSet<Localidad> Localidad { get; set; }
    public virtual DbSet<OdontogramaPieza> OdontogramaPieza { get; set; }
    public virtual DbSet<Paciente> Paciente { get; set; }
    public virtual DbSet<Pago> Pago { get; set; }
    public virtual DbSet<Periodontograma> Periodontograma { get; set; }
    public virtual DbSet<PeriodontogramaPieza> PeriodontogramaPieza { get; set; }
    public virtual DbSet<Presupuesto> Presupuesto { get; set; }
    public virtual DbSet<PresupuestoDetalle> PresupuestoDetalle { get; set; }
    public virtual DbSet<Profesional> Profesional { get; set; }
    public virtual DbSet<Provincia> Provincia { get; set; }
    public virtual DbSet<Tratamiento> Tratamiento { get; set; }
    public virtual DbSet<Turno> Turno { get; set; }
  }
}

