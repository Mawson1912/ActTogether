// Fetch unread message count from API and update nav badge
function loadUnreadCount() {
    fetch('/api/UsersApi/unread')
        .then(response => response.json())
        .then(data => {
            var badge = document.getElementById('unread-badge');
            if (badge && data.unreadCount > 0) {
                badge.textContent = data.unreadCount;
                badge.style.display = 'inline';
            }
        })
        .catch(error => console.log('Could not load unread count'));
}

// Fetch pending friend request count from API and update nav badge
function loadPendingCount() {
    fetch('/api/FriendshipsApi/pendingcount')
        .then(response => response.json())
        .then(data => {
            var badge = document.getElementById('pending-badge');
            if (badge && data.pendingCount > 0) {
                badge.textContent = data.pendingCount;
                badge.style.display = 'inline';
            }
        })
        .catch(error => console.log('Could not load pending count'));
}

// Delete a post via API and remove the card from the page
function deletePost(postId, button) {
    if (!confirm('Are you sure you want to delete this post?')) return;

    fetch('/api/PostsApi/' + postId, { method: 'DELETE' })
        .then(response => {
            if (response.ok) {
                var card = button.closest('.card');
                card.remove();
            } else {
                alert('Could not delete post.');
            }
        })
        .catch(error => alert('Error deleting post.'));
}

// Run on page load
document.addEventListener('DOMContentLoaded', function () {
    loadUnreadCount();
    loadPendingCount();
});
