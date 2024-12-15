using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    public class SessionRepository : IDisposable
    {
        private readonly Sales_DBEntities _context;

        public SessionRepository()
        {
            _context = new Sales_DBEntities();
        }

        // Crear una nueva sesión
        public Sessions CreateSession(Sessions session)
        {
            session.CreatedAt = DateTime.Now;
            session.IsActive = true;
            _context.Sessions.Add(session);
            _context.SaveChanges();
            return session;
        }

        // Obtener sesión activa por token
        public Sessions GetSessionByToken(string sessionToken)
        {
            return _context.Sessions
                           .FirstOrDefault(s => s.SessionToken == sessionToken && s.IsActive && s.ExpiresAt > DateTime.Now);
        }

        // Finalizar sesión por token
        public void EndSession(string sessionToken)
        {
            var session = _context.Sessions.FirstOrDefault(s => s.SessionToken == sessionToken);
            if (session != null)
            {
                session.IsActive = false;
                _context.SaveChanges();
            }
        }

        // Finalizar todas las sesiones de un usuario
        public void EndAllSessions(int userId)
        {
            var sessions = _context.Sessions.Where(s => s.UserId == userId && s.IsActive).ToList();
            foreach (var session in sessions)
            {
                session.IsActive = false;
            }
            _context.SaveChanges();
        }
        public void UpdateSession(Sessions session)
        {
            var existingSession = _context.Sessions.FirstOrDefault(s => s.SessionId == session.SessionId);
            if (existingSession != null)
            {
                existingSession.IsActive = session.IsActive;
                _context.SaveChanges();
            }
        }


        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
