using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Ecommerce.Modelo;
using Ecommerce.DTO;
using Ecommerce.Repositorio.Contrato;
using Ecommerce.Servicio.Contrato;
using AutoMapper;
using System.Security.Claims;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using Ecommerce.Utilidades;


namespace Ecommerce.Servicio.Implementacion
{
    public class UsuarioServicio : IUsuarioServicio
    {
        private readonly IGenericoRepositorio<Usuario> _modeloRepositorio;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;

        public UsuarioServicio(IGenericoRepositorio<Usuario> modeloRepositorio, IMapper mapper, IConfiguration configuration)
        {
            _modeloRepositorio = modeloRepositorio;
            _mapper = mapper;
            _configuration = configuration;
        }

        public async Task<SesionDTO> Autorizacion(LoginDTO modelo)
        {
            try 
            {                
                var consulta = _modeloRepositorio.Consultar(p => p.Correo == modelo.Correo);
                var fromDbModelo = await consulta.FirstOrDefaultAsync();

                if (fromDbModelo != null)
                {

                    var salt = fromDbModelo.Salt;
                    var claveEncriptada = EncriptacionHelper.EncriptarContraseña(modelo.Clave!, fromDbModelo.Salt!);

                    if (fromDbModelo.Clave == claveEncriptada)
                    {
                        var claims = new[]
                        {
                            new Claim(ClaimTypes.Name, fromDbModelo.NombreCompleto),
                            new Claim(ClaimTypes.Role, fromDbModelo.Rol)
                        };
                        
                        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));
                        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                        var token = new JwtSecurityToken(
                            issuer: _configuration["Jwt:Issuer"],
                            audience: _configuration["Jwt:Audience"],
                            claims: claims,
                            notBefore: DateTime.Now,
                            expires: DateTime.Now.AddMinutes(Convert.ToDouble(_configuration["Jwt:ExpirationInMinutes"])),
                            signingCredentials: creds
                        );

                        var sesion = _mapper.Map<SesionDTO>(fromDbModelo);
                        sesion.Token = new JwtSecurityTokenHandler().WriteToken(token);

                        return sesion;
                    }
                    else
                    {
                        throw new TaskCanceledException("Contraseña incorrecta");
                    }                    
                }
                    
                else
                    throw new TaskCanceledException("El correo no se encuentra registrado");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<UsuarioDTO> Crear(UsuarioDTO modelo)
        {
            try
            {
                var dbModelo = _mapper.Map<Usuario>(modelo);

                var salt = EncriptacionHelper.GenerarSalt();

                Usuario usuarioDef = new Usuario
                {
                    IdUsuario = dbModelo.IdUsuario,
                    NombreCompleto = dbModelo.NombreCompleto,
                    Correo = dbModelo.Correo,
                    Salt = salt,
                    Clave = EncriptacionHelper.EncriptarContraseña(dbModelo.Clave!, salt!),
                    Rol = dbModelo.Rol,
                    FechaCreacion = dbModelo.FechaCreacion
                };


                var rspModelo = await _modeloRepositorio.Crear(usuarioDef);            

                if (rspModelo.IdUsuario != 0)
                    return _mapper.Map<UsuarioDTO>(rspModelo);
                else
                    throw new TaskCanceledException("No se pudo crear");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<bool> Editar(UsuarioDTO modelo)
        {
            try
            {
                var consulta = _modeloRepositorio.Consultar(p => p.IdUsuario == modelo.IdUsuario);
                var fromDbModelo = await consulta.FirstOrDefaultAsync();

                if (fromDbModelo != null)
                {
                    fromDbModelo.NombreCompleto = modelo.NombreCompleto;
                    fromDbModelo.Correo = modelo.Correo;
                    fromDbModelo.Clave = modelo.Clave;
                    var respuesta = await _modeloRepositorio.Editar(fromDbModelo);

                    if (!respuesta)
                        throw new TaskCanceledException("No se pudo editar");
                    return respuesta;
                }
                else
                {
                    throw new TaskCanceledException("No se encontró el usuario a editar");
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<bool> Eliminar(int id)
        {
            try
            {
                var consulta = _modeloRepositorio.Consultar(p => p.IdUsuario == id);
                var fromDbModelo = await consulta.FirstOrDefaultAsync();

                if (fromDbModelo != null)
                {
                    var respuesta = await _modeloRepositorio.Eliminar(fromDbModelo);
                    if (!respuesta)
                        throw new TaskCanceledException("No se pudo eliminar");
                    return respuesta;
                }
                else
                {
                    throw new TaskCanceledException("No se encontró el usuario a eliminar");
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<List<UsuarioDTO>> Lista(string rol, string buscar)
        {
            try
            {
                var consulta = _modeloRepositorio.Consultar(p =>
                p.Rol == rol && string.Concat(p.NombreCompleto.ToLower(),p.Correo.ToLower()).Contains(buscar.ToLower()));

                List<UsuarioDTO> lista = _mapper.Map<List<UsuarioDTO>>(await consulta.ToListAsync());
                return lista;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<UsuarioDTO> Obtener(int id)
        {
            try
            {
                var consulta = _modeloRepositorio.Consultar(p => p.IdUsuario == id);
                var fromDbModelo = await consulta.FirstOrDefaultAsync();


                if (fromDbModelo != null)
                    return _mapper.Map<UsuarioDTO>(fromDbModelo);
                else
                    throw new TaskCanceledException("No se encontró el usuario solicitado");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
