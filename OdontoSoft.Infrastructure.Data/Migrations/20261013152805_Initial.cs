using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace OdontoSoft.Infrastructure.Data.Migrations
{
  /// <inheritdoc />
  public partial class Initial : Migration
  {
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
      migrationBuilder.CreateTable(
          name: "estado_turno",
          columns: table => new
          {
            id = table.Column<int>(type: "integer", nullable: false)
                  .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
            nombre = table.Column<string>(type: "text", nullable: false),
            color_hex = table.Column<string>(type: "text", nullable: false),
            disabled = table.Column<bool>(type: "boolean", nullable: false)
          },
          constraints: table =>
          {
            table.PrimaryKey("pk_estado_turno", x => x.id);
          });

      migrationBuilder.CreateTable(
          name: "provincia",
          columns: table => new
          {
            id = table.Column<int>(type: "integer", nullable: false)
                  .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
            descripcion = table.Column<string>(type: "text", nullable: false),
            disabled = table.Column<bool>(type: "boolean", nullable: false)
          },
          constraints: table =>
          {
            table.PrimaryKey("pk_provincia", x => x.id);
          });

      migrationBuilder.CreateTable(
          name: "tratamiento",
          columns: table => new
          {
            id = table.Column<int>(type: "integer", nullable: false)
                  .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
            codigo = table.Column<string>(type: "text", nullable: true),
            nombre = table.Column<string>(type: "text", nullable: true),
            descripcion = table.Column<string>(type: "text", nullable: true),
            disabled = table.Column<bool>(type: "boolean", nullable: false)
          },
          constraints: table =>
          {
            table.PrimaryKey("pk_tratamiento", x => x.id);
          });

      migrationBuilder.CreateTable(
          name: "ubicacion",
          columns: table => new
          {
            id = table.Column<int>(type: "integer", nullable: false)
                  .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
            nombre = table.Column<string>(type: "text", nullable: false),
            direccion = table.Column<string>(type: "text", nullable: false),
            codigo_postal = table.Column<string>(type: "text", nullable: true),
            ciudad = table.Column<string>(type: "text", nullable: false),
            provincia = table.Column<string>(type: "text", nullable: true),
            telefono = table.Column<string>(type: "text", nullable: true),
            email = table.Column<string>(type: "text", nullable: true),
            disabled = table.Column<bool>(type: "boolean", nullable: false),
            created_user_id = table.Column<int>(type: "integer", nullable: true),
            created_user_name = table.Column<string>(type: "text", nullable: true),
            created_date = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
            updated_user_id = table.Column<int>(type: "integer", nullable: true),
            updated_user_name = table.Column<string>(type: "text", nullable: true),
            updated_date = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
          },
          constraints: table =>
          {
            table.PrimaryKey("pk_ubicacion", x => x.id);
          });

      migrationBuilder.CreateTable(
          name: "localidad",
          columns: table => new
          {
            id = table.Column<int>(type: "integer", nullable: false)
                  .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
            descripcion = table.Column<string>(type: "text", nullable: false),
            provincia_id = table.Column<int>(type: "integer", nullable: false),
            disabled = table.Column<bool>(type: "boolean", nullable: false)
          },
          constraints: table =>
          {
            table.PrimaryKey("pk_localidad", x => x.id);
            table.ForeignKey(
                      name: "fk_localidad_provincia_provincia_id",
                      column: x => x.provincia_id,
                      principalTable: "provincia",
                      principalColumn: "id",
                      onDelete: ReferentialAction.Restrict);
          });

      migrationBuilder.CreateTable(
          name: "tratamiento_precio",
          columns: table => new
          {
            id = table.Column<int>(type: "integer", nullable: false)
                  .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
            tratamiento_id = table.Column<int>(type: "integer", nullable: false),
            ubicacion_id = table.Column<int>(type: "integer", nullable: false),
            precio = table.Column<decimal>(type: "numeric", nullable: false),
            disabled = table.Column<bool>(type: "boolean", nullable: false),
            created_user_id = table.Column<int>(type: "integer", nullable: true),
            created_user_name = table.Column<string>(type: "text", nullable: true),
            created_date = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
            updated_user_id = table.Column<int>(type: "integer", nullable: true),
            updated_user_name = table.Column<string>(type: "text", nullable: true),
            updated_date = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
          },
          constraints: table =>
          {
            table.PrimaryKey("pk_tratamiento_precio", x => x.id);
            table.ForeignKey(
                      name: "fk_tratamiento_precio_tratamiento_tratamiento_id",
                      column: x => x.tratamiento_id,
                      principalTable: "tratamiento",
                      principalColumn: "id",
                      onDelete: ReferentialAction.Restrict);
            table.ForeignKey(
                      name: "fk_tratamiento_precio_ubicacion_ubicacion_id",
                      column: x => x.ubicacion_id,
                      principalTable: "ubicacion",
                      principalColumn: "id",
                      onDelete: ReferentialAction.Restrict);
          });

      migrationBuilder.CreateTable(
          name: "paciente",
          columns: table => new
          {
            id = table.Column<int>(type: "integer", nullable: false)
                  .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
            nombre = table.Column<string>(type: "text", nullable: false),
            apellido = table.Column<string>(type: "text", nullable: false),
            dni = table.Column<string>(type: "text", nullable: false),
            telefono = table.Column<string>(type: "text", nullable: true),
            celular = table.Column<string>(type: "text", nullable: true),
            fecha_nacimiento = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
            domicilio_calle = table.Column<string>(type: "text", nullable: true),
            domicilio_numero = table.Column<int>(type: "integer", nullable: true),
            provincia_id = table.Column<int>(type: "integer", nullable: true),
            localidad_id = table.Column<int>(type: "integer", nullable: true),
            email = table.Column<string>(type: "text", nullable: true),
            disabled = table.Column<bool>(type: "boolean", nullable: false),
            created_user_id = table.Column<int>(type: "integer", nullable: true),
            created_user_name = table.Column<string>(type: "text", nullable: true),
            created_date = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
            updated_user_id = table.Column<int>(type: "integer", nullable: true),
            updated_user_name = table.Column<string>(type: "text", nullable: true),
            updated_date = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
          },
          constraints: table =>
          {
            table.PrimaryKey("pk_paciente", x => x.id);
            table.ForeignKey(
                      name: "fk_paciente_localidad_localidad_id",
                      column: x => x.localidad_id,
                      principalTable: "localidad",
                      principalColumn: "id",
                      onDelete: ReferentialAction.Restrict);
            table.ForeignKey(
                      name: "fk_paciente_provincia_provincia_id",
                      column: x => x.provincia_id,
                      principalTable: "provincia",
                      principalColumn: "id",
                      onDelete: ReferentialAction.Restrict);
          });

      migrationBuilder.CreateTable(
          name: "profesional",
          columns: table => new
          {
            id = table.Column<int>(type: "integer", nullable: false)
                  .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
            nombre = table.Column<string>(type: "text", nullable: true),
            apellido = table.Column<string>(type: "text", nullable: true),
            dni = table.Column<string>(type: "text", nullable: true),
            celular = table.Column<string>(type: "text", nullable: true),
            fecha_nacimiento = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
            domicilio = table.Column<string>(type: "text", nullable: true),
            provincia_id = table.Column<int>(type: "integer", nullable: true),
            localidad_id = table.Column<int>(type: "integer", nullable: true),
            email = table.Column<string>(type: "text", nullable: true),
            cuit = table.Column<string>(type: "text", nullable: true),
            disabled = table.Column<bool>(type: "boolean", nullable: false),
            created_user_id = table.Column<int>(type: "integer", nullable: true),
            created_user_name = table.Column<string>(type: "text", nullable: true),
            created_date = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
            updated_user_id = table.Column<int>(type: "integer", nullable: true),
            updated_user_name = table.Column<string>(type: "text", nullable: true),
            updated_date = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
          },
          constraints: table =>
          {
            table.PrimaryKey("pk_profesional", x => x.id);
            table.ForeignKey(
                      name: "fk_profesional_localidad_localidad_id",
                      column: x => x.localidad_id,
                      principalTable: "localidad",
                      principalColumn: "id",
                      onDelete: ReferentialAction.Restrict);
            table.ForeignKey(
                      name: "fk_profesional_provincia_provincia_id",
                      column: x => x.provincia_id,
                      principalTable: "provincia",
                      principalColumn: "id",
                      onDelete: ReferentialAction.Restrict);
          });

      migrationBuilder.CreateTable(
          name: "historia_clinica",
          columns: table => new
          {
            id = table.Column<int>(type: "integer", nullable: false)
                  .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
            paciente_id = table.Column<int>(type: "integer", nullable: false),
            alergias = table.Column<string>(type: "text", nullable: true),
            medicamentos = table.Column<string>(type: "text", nullable: true),
            antecedentes_medicos = table.Column<string>(type: "text", nullable: true),
            observaciones = table.Column<string>(type: "text", nullable: true),
            disabled = table.Column<bool>(type: "boolean", nullable: false),
            created_user_id = table.Column<int>(type: "integer", nullable: true),
            created_user_name = table.Column<string>(type: "text", nullable: true),
            created_date = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
            updated_user_id = table.Column<int>(type: "integer", nullable: true),
            updated_user_name = table.Column<string>(type: "text", nullable: true),
            updated_date = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
          },
          constraints: table =>
          {
            table.PrimaryKey("pk_historia_clinica", x => x.id);
            table.ForeignKey(
                      name: "fk_historia_clinica_paciente_paciente_id",
                      column: x => x.paciente_id,
                      principalTable: "paciente",
                      principalColumn: "id",
                      onDelete: ReferentialAction.Restrict);
          });

      migrationBuilder.CreateTable(
          name: "presupuesto",
          columns: table => new
          {
            id = table.Column<int>(type: "integer", nullable: false)
                  .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
            paciente_id = table.Column<int>(type: "integer", nullable: false),
            profesional_id = table.Column<int>(type: "integer", nullable: true),
            fecha = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
            observaciones = table.Column<string>(type: "text", nullable: true),
            total = table.Column<decimal>(type: "numeric", nullable: false),
            estado = table.Column<string>(type: "text", nullable: false),
            disabled = table.Column<bool>(type: "boolean", nullable: false),
            created_user_id = table.Column<int>(type: "integer", nullable: true),
            created_user_name = table.Column<string>(type: "text", nullable: true),
            created_date = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
            updated_user_id = table.Column<int>(type: "integer", nullable: true),
            updated_user_name = table.Column<string>(type: "text", nullable: true),
            updated_date = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
          },
          constraints: table =>
          {
            table.PrimaryKey("pk_presupuesto", x => x.id);
            table.ForeignKey(
                      name: "fk_presupuesto_paciente_paciente_id",
                      column: x => x.paciente_id,
                      principalTable: "paciente",
                      principalColumn: "id",
                      onDelete: ReferentialAction.Restrict);
            table.ForeignKey(
                      name: "fk_presupuesto_profesional_profesional_id",
                      column: x => x.profesional_id,
                      principalTable: "profesional",
                      principalColumn: "id",
                      onDelete: ReferentialAction.Restrict);
          });

      migrationBuilder.CreateTable(
          name: "turno",
          columns: table => new
          {
            id = table.Column<int>(type: "integer", nullable: false)
                  .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
            paciente_id = table.Column<int>(type: "integer", nullable: false),
            profesional_id = table.Column<int>(type: "integer", nullable: false),
            fecha = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
            hora_inicio = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
            hora_fin = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
            observacion = table.Column<string>(type: "text", nullable: true),
            tratamiento_id = table.Column<int>(type: "integer", nullable: true),
            estado_turno_id = table.Column<int>(type: "integer", nullable: true),
            ubicacion_id = table.Column<int>(type: "integer", nullable: true),
            disabled = table.Column<bool>(type: "boolean", nullable: false),
            created_user_id = table.Column<int>(type: "integer", nullable: true),
            created_user_name = table.Column<string>(type: "text", nullable: true),
            created_date = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
            updated_user_id = table.Column<int>(type: "integer", nullable: true),
            updated_user_name = table.Column<string>(type: "text", nullable: true),
            updated_date = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
          },
          constraints: table =>
          {
            table.PrimaryKey("pk_turno", x => x.id);
            table.ForeignKey(
                      name: "fk_turno_estado_turno_estado_turno_id",
                      column: x => x.estado_turno_id,
                      principalTable: "estado_turno",
                      principalColumn: "id",
                      onDelete: ReferentialAction.Restrict);
            table.ForeignKey(
                      name: "fk_turno_paciente_paciente_id",
                      column: x => x.paciente_id,
                      principalTable: "paciente",
                      principalColumn: "id",
                      onDelete: ReferentialAction.Restrict);
            table.ForeignKey(
                      name: "fk_turno_profesional_profesional_id",
                      column: x => x.profesional_id,
                      principalTable: "profesional",
                      principalColumn: "id",
                      onDelete: ReferentialAction.Restrict);
            table.ForeignKey(
                      name: "fk_turno_tratamiento_tratamiento_id",
                      column: x => x.tratamiento_id,
                      principalTable: "tratamiento",
                      principalColumn: "id",
                      onDelete: ReferentialAction.Restrict);
            table.ForeignKey(
                      name: "fk_turno_ubicacion_ubicacion_id",
                      column: x => x.ubicacion_id,
                      principalTable: "ubicacion",
                      principalColumn: "id",
                      onDelete: ReferentialAction.Restrict);
          });

      migrationBuilder.CreateTable(
          name: "imagen_paciente",
          columns: table => new
          {
            id = table.Column<int>(type: "integer", nullable: false)
                  .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
            historia_clinica_id = table.Column<int>(type: "integer", nullable: false),
            nombre_archivo = table.Column<string>(type: "text", nullable: false),
            ruta_archivo = table.Column<string>(type: "text", nullable: false),
            mime_type = table.Column<string>(type: "text", nullable: false),
            tamano_bytes = table.Column<long>(type: "bigint", nullable: false),
            descripcion = table.Column<string>(type: "text", nullable: true),
            fecha_subida = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
            disabled = table.Column<bool>(type: "boolean", nullable: false),
            created_user_id = table.Column<int>(type: "integer", nullable: true),
            created_user_name = table.Column<string>(type: "text", nullable: true),
            created_date = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
            updated_user_id = table.Column<int>(type: "integer", nullable: true),
            updated_user_name = table.Column<string>(type: "text", nullable: true),
            updated_date = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
          },
          constraints: table =>
          {
            table.PrimaryKey("pk_imagen_paciente", x => x.id);
            table.ForeignKey(
                      name: "fk_imagen_paciente_historia_clinica_historia_clinica_id",
                      column: x => x.historia_clinica_id,
                      principalTable: "historia_clinica",
                      principalColumn: "id",
                      onDelete: ReferentialAction.Restrict);
          });

      migrationBuilder.CreateTable(
          name: "odontograma_pieza",
          columns: table => new
          {
            id = table.Column<int>(type: "integer", nullable: false)
                  .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
            historia_clinica_id = table.Column<int>(type: "integer", nullable: false),
            numero_pieza = table.Column<int>(type: "integer", nullable: false),
            estado = table.Column<string>(type: "text", nullable: true),
            observacion = table.Column<string>(type: "text", nullable: true),
            disabled = table.Column<bool>(type: "boolean", nullable: false),
            created_user_id = table.Column<int>(type: "integer", nullable: true),
            created_user_name = table.Column<string>(type: "text", nullable: true),
            created_date = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
            updated_user_id = table.Column<int>(type: "integer", nullable: true),
            updated_user_name = table.Column<string>(type: "text", nullable: true),
            updated_date = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
          },
          constraints: table =>
          {
            table.PrimaryKey("pk_odontograma_pieza", x => x.id);
            table.ForeignKey(
                      name: "fk_odontograma_pieza_historia_clinica_historia_clinica_id",
                      column: x => x.historia_clinica_id,
                      principalTable: "historia_clinica",
                      principalColumn: "id",
                      onDelete: ReferentialAction.Restrict);
          });

      migrationBuilder.CreateTable(
          name: "periodontograma",
          columns: table => new
          {
            id = table.Column<int>(type: "integer", nullable: false)
                  .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
            historia_clinica_id = table.Column<int>(type: "integer", nullable: false),
            fecha_registro = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
            observaciones = table.Column<string>(type: "text", nullable: true),
            disabled = table.Column<bool>(type: "boolean", nullable: false),
            created_user_id = table.Column<int>(type: "integer", nullable: true),
            created_user_name = table.Column<string>(type: "text", nullable: true),
            created_date = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
            updated_user_id = table.Column<int>(type: "integer", nullable: true),
            updated_user_name = table.Column<string>(type: "text", nullable: true),
            updated_date = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
          },
          constraints: table =>
          {
            table.PrimaryKey("pk_periodontograma", x => x.id);
            table.ForeignKey(
                      name: "fk_periodontograma_historia_clinica_historia_clinica_id",
                      column: x => x.historia_clinica_id,
                      principalTable: "historia_clinica",
                      principalColumn: "id",
                      onDelete: ReferentialAction.Restrict);
          });

      migrationBuilder.CreateTable(
          name: "pago",
          columns: table => new
          {
            id = table.Column<int>(type: "integer", nullable: false)
                  .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
            presupuesto_id = table.Column<int>(type: "integer", nullable: false),
            fecha = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
            monto = table.Column<decimal>(type: "numeric", nullable: false),
            metodo_pago = table.Column<string>(type: "text", nullable: false),
            observaciones = table.Column<string>(type: "text", nullable: true),
            disabled = table.Column<bool>(type: "boolean", nullable: false),
            created_user_id = table.Column<int>(type: "integer", nullable: true),
            created_user_name = table.Column<string>(type: "text", nullable: true),
            created_date = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
            updated_user_id = table.Column<int>(type: "integer", nullable: true),
            updated_user_name = table.Column<string>(type: "text", nullable: true),
            updated_date = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
          },
          constraints: table =>
          {
            table.PrimaryKey("pk_pago", x => x.id);
            table.ForeignKey(
                      name: "fk_pago_presupuesto_presupuesto_id",
                      column: x => x.presupuesto_id,
                      principalTable: "presupuesto",
                      principalColumn: "id",
                      onDelete: ReferentialAction.Restrict);
          });

      migrationBuilder.CreateTable(
          name: "presupuesto_detalle",
          columns: table => new
          {
            id = table.Column<int>(type: "integer", nullable: false)
                  .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
            presupuesto_id = table.Column<int>(type: "integer", nullable: false),
            tratamiento_id = table.Column<int>(type: "integer", nullable: false),
            numero_pieza = table.Column<int>(type: "integer", nullable: true),
            cantidad = table.Column<int>(type: "integer", nullable: false),
            precio_unitario = table.Column<decimal>(type: "numeric", nullable: false),
            subtotal = table.Column<decimal>(type: "numeric", nullable: false),
            disabled = table.Column<bool>(type: "boolean", nullable: false),
            created_user_id = table.Column<int>(type: "integer", nullable: true),
            created_user_name = table.Column<string>(type: "text", nullable: true),
            created_date = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
            updated_user_id = table.Column<int>(type: "integer", nullable: true),
            updated_user_name = table.Column<string>(type: "text", nullable: true),
            updated_date = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
          },
          constraints: table =>
          {
            table.PrimaryKey("pk_presupuesto_detalle", x => x.id);
            table.ForeignKey(
                      name: "fk_presupuesto_detalle_presupuesto_presupuesto_id",
                      column: x => x.presupuesto_id,
                      principalTable: "presupuesto",
                      principalColumn: "id",
                      onDelete: ReferentialAction.Restrict);
            table.ForeignKey(
                      name: "fk_presupuesto_detalle_tratamiento_tratamiento_id",
                      column: x => x.tratamiento_id,
                      principalTable: "tratamiento",
                      principalColumn: "id",
                      onDelete: ReferentialAction.Restrict);
          });

      migrationBuilder.CreateTable(
          name: "ficha_tratamiento",
          columns: table => new
          {
            id = table.Column<int>(type: "integer", nullable: false)
                  .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
            historia_clinica_id = table.Column<int>(type: "integer", nullable: false),
            turno_id = table.Column<int>(type: "integer", nullable: true),
            tratamiento_id = table.Column<int>(type: "integer", nullable: false),
            numero_pieza = table.Column<int>(type: "integer", nullable: true),
            descripcion = table.Column<string>(type: "text", nullable: true),
            observaciones = table.Column<string>(type: "text", nullable: true),
            fecha_realizacion = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
            disabled = table.Column<bool>(type: "boolean", nullable: false),
            created_user_id = table.Column<int>(type: "integer", nullable: true),
            created_user_name = table.Column<string>(type: "text", nullable: true),
            created_date = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
            updated_user_id = table.Column<int>(type: "integer", nullable: true),
            updated_user_name = table.Column<string>(type: "text", nullable: true),
            updated_date = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
          },
          constraints: table =>
          {
            table.PrimaryKey("pk_ficha_tratamiento", x => x.id);
            table.ForeignKey(
                      name: "fk_ficha_tratamiento_historia_clinica_historia_clinica_id",
                      column: x => x.historia_clinica_id,
                      principalTable: "historia_clinica",
                      principalColumn: "id",
                      onDelete: ReferentialAction.Restrict);
            table.ForeignKey(
                      name: "fk_ficha_tratamiento_tratamiento_tratamiento_id",
                      column: x => x.tratamiento_id,
                      principalTable: "tratamiento",
                      principalColumn: "id",
                      onDelete: ReferentialAction.Restrict);
            table.ForeignKey(
                      name: "fk_ficha_tratamiento_turno_turno_id",
                      column: x => x.turno_id,
                      principalTable: "turno",
                      principalColumn: "id",
                      onDelete: ReferentialAction.Restrict);
          });

      migrationBuilder.CreateTable(
          name: "periodontograma_pieza",
          columns: table => new
          {
            id = table.Column<int>(type: "integer", nullable: false)
                  .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
            periodontograma_id = table.Column<int>(type: "integer", nullable: false),
            numero_pieza = table.Column<int>(type: "integer", nullable: false),
            estado = table.Column<string>(type: "text", nullable: true),
            sangrado = table.Column<bool>(type: "boolean", nullable: false),
            supuracion = table.Column<bool>(type: "boolean", nullable: false),
            placa = table.Column<bool>(type: "boolean", nullable: false),
            bolsa_periodontal = table.Column<decimal>(type: "numeric", nullable: true),
            recesion = table.Column<decimal>(type: "numeric", nullable: true),
            observacion = table.Column<string>(type: "text", nullable: true),
            disabled = table.Column<bool>(type: "boolean", nullable: false),
            created_user_id = table.Column<int>(type: "integer", nullable: true),
            created_user_name = table.Column<string>(type: "text", nullable: true),
            created_date = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
            updated_user_id = table.Column<int>(type: "integer", nullable: true),
            updated_user_name = table.Column<string>(type: "text", nullable: true),
            updated_date = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
          },
          constraints: table =>
          {
            table.PrimaryKey("pk_periodontograma_pieza", x => x.id);
            table.ForeignKey(
                      name: "fk_periodontograma_pieza_periodontograma_periodontograma_id",
                      column: x => x.periodontograma_id,
                      principalTable: "periodontograma",
                      principalColumn: "id",
                      onDelete: ReferentialAction.Restrict);
          });

      migrationBuilder.CreateIndex(
          name: "ix_ficha_tratamiento_historia_clinica_id",
          table: "ficha_tratamiento",
          column: "historia_clinica_id");

      migrationBuilder.CreateIndex(
          name: "ix_ficha_tratamiento_tratamiento_id",
          table: "ficha_tratamiento",
          column: "tratamiento_id");

      migrationBuilder.CreateIndex(
          name: "ix_ficha_tratamiento_turno_id",
          table: "ficha_tratamiento",
          column: "turno_id");

      migrationBuilder.CreateIndex(
          name: "ix_historia_clinica_paciente_id",
          table: "historia_clinica",
          column: "paciente_id");

      migrationBuilder.CreateIndex(
          name: "ix_imagen_paciente_historia_clinica_id",
          table: "imagen_paciente",
          column: "historia_clinica_id");

      migrationBuilder.CreateIndex(
          name: "ix_localidad_provincia_id",
          table: "localidad",
          column: "provincia_id");

      migrationBuilder.CreateIndex(
          name: "ix_odontograma_pieza_historia_clinica_id",
          table: "odontograma_pieza",
          column: "historia_clinica_id");

      migrationBuilder.CreateIndex(
          name: "ix_paciente_localidad_id",
          table: "paciente",
          column: "localidad_id");

      migrationBuilder.CreateIndex(
          name: "ix_paciente_provincia_id",
          table: "paciente",
          column: "provincia_id");

      migrationBuilder.CreateIndex(
          name: "ix_pago_presupuesto_id",
          table: "pago",
          column: "presupuesto_id");

      migrationBuilder.CreateIndex(
          name: "ix_periodontograma_historia_clinica_id",
          table: "periodontograma",
          column: "historia_clinica_id");

      migrationBuilder.CreateIndex(
          name: "ix_periodontograma_pieza_periodontograma_id",
          table: "periodontograma_pieza",
          column: "periodontograma_id");

      migrationBuilder.CreateIndex(
          name: "ix_presupuesto_paciente_id",
          table: "presupuesto",
          column: "paciente_id");

      migrationBuilder.CreateIndex(
          name: "ix_presupuesto_profesional_id",
          table: "presupuesto",
          column: "profesional_id");

      migrationBuilder.CreateIndex(
          name: "ix_presupuesto_detalle_presupuesto_id",
          table: "presupuesto_detalle",
          column: "presupuesto_id");

      migrationBuilder.CreateIndex(
          name: "ix_presupuesto_detalle_tratamiento_id",
          table: "presupuesto_detalle",
          column: "tratamiento_id");

      migrationBuilder.CreateIndex(
          name: "ix_profesional_localidad_id",
          table: "profesional",
          column: "localidad_id");

      migrationBuilder.CreateIndex(
          name: "ix_profesional_provincia_id",
          table: "profesional",
          column: "provincia_id");

      migrationBuilder.CreateIndex(
          name: "ix_tratamiento_precio_tratamiento_id",
          table: "tratamiento_precio",
          column: "tratamiento_id");

      migrationBuilder.CreateIndex(
          name: "ix_tratamiento_precio_ubicacion_id",
          table: "tratamiento_precio",
          column: "ubicacion_id");

      migrationBuilder.CreateIndex(
          name: "ix_turno_estado_turno_id",
          table: "turno",
          column: "estado_turno_id");

      migrationBuilder.CreateIndex(
          name: "ix_turno_paciente_id",
          table: "turno",
          column: "paciente_id");

      migrationBuilder.CreateIndex(
          name: "ix_turno_profesional_id",
          table: "turno",
          column: "profesional_id");

      migrationBuilder.CreateIndex(
          name: "ix_turno_tratamiento_id",
          table: "turno",
          column: "tratamiento_id");

      migrationBuilder.CreateIndex(
          name: "ix_turno_ubicacion_id",
          table: "turno",
          column: "ubicacion_id");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
      migrationBuilder.DropTable(
          name: "ficha_tratamiento");

      migrationBuilder.DropTable(
          name: "imagen_paciente");

      migrationBuilder.DropTable(
          name: "odontograma_pieza");

      migrationBuilder.DropTable(
          name: "pago");

      migrationBuilder.DropTable(
          name: "periodontograma_pieza");

      migrationBuilder.DropTable(
          name: "presupuesto_detalle");

      migrationBuilder.DropTable(
          name: "tratamiento_precio");

      migrationBuilder.DropTable(
          name: "turno");

      migrationBuilder.DropTable(
          name: "periodontograma");

      migrationBuilder.DropTable(
          name: "presupuesto");

      migrationBuilder.DropTable(
          name: "estado_turno");

      migrationBuilder.DropTable(
          name: "tratamiento");

      migrationBuilder.DropTable(
          name: "ubicacion");

      migrationBuilder.DropTable(
          name: "historia_clinica");

      migrationBuilder.DropTable(
          name: "profesional");

      migrationBuilder.DropTable(
          name: "paciente");

      migrationBuilder.DropTable(
          name: "localidad");

      migrationBuilder.DropTable(
          name: "provincia");
    }
  }
}
