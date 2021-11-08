
using System.Collections.Generic;
using System.Threading.Tasks;
using TicketControl.Data.Contracts;
using TicketControl.Data.Models;

namespace TicketControl.BLL.Managers
{
    public class CommentManager
    {
        private readonly ICommentRepository _commentRepository;

        public CommentManager(ICommentRepository commentRepository)
        {
            _commentRepository = commentRepository;
        }

        public async Task<ICollection<Comment>> GetAllComments()
        {
            return await _commentRepository.GetAllAsync();
        }
        public async Task<ICollection<Comment>> GetCommentsForProject(int projectId)
        {
            return await _commentRepository.GetCommentsByProject(projectId);
        }
        public async Task<ICollection<Comment>> GetCommentsForTicket(int ticketId)
        {
            return await _commentRepository.GetCommentsByTicket(ticketId);
        }
        public async Task<ICollection<Comment>> GetCommentsForUser(string userId)
        {
            return await _commentRepository.GetCommentsByUser(userId);
        }

        public async Task<bool> CreateAsync(Comment comment)
        {
            return await _commentRepository.CreateAsync(comment);
        }

        public async Task<Comment> GetByIdAsync(int id)
        {
            return await _commentRepository.GetByIdAsync(id);
        }

        public async Task<bool> DeleteAsync(Comment comment)
        {
            return await _commentRepository.DeleteAsync(comment);
        }
        public async Task<bool> UpdateAsync(Comment comment)
        {
            return await _commentRepository.UpdateAsync(comment);
        }
    }
}
