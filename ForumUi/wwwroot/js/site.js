// site.js - Beğeni işlemleri için AJAX fonksiyonları

function likeReply(replyId, topicId) {
    fetch('/Topics/LikeReply', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/x-www-form-urlencoded',
            'RequestVerificationToken': document.querySelector('input[name="__RequestVerificationToken"]')?.value || ''
        },
        body: `replyId=${replyId}&topicId=${topicId}`
    })
        .then(response => response.json())
        .then(data => {
            if (data.success) {
                // Beğeni sayısını güncelle
                const likeCountElement = document.getElementById(`reply-like-count-${replyId}`);
                if (likeCountElement) {
                    likeCountElement.textContent = data.likeCount;
                }

                // Buton rengini değiştir
                const likeButton = document.getElementById(`reply-like-btn-${replyId}`);
                if (likeButton) {
                    if (data.liked) {
                        likeButton.classList.remove('btn-outline-primary');
                        likeButton.classList.add('btn-primary');
                    } else {
                        likeButton.classList.remove('btn-primary');
                        likeButton.classList.add('btn-outline-primary');
                    }
                }

                // Başarı mesajı göster
                showMessage('Beğeni işlemi başarılı!', 'success');
            } else {
                showMessage(data.message || 'Beğeni işlemi başarısız oldu.', 'error');
            }
        })
        .catch(error => {
            console.error('Error:', error);
            showMessage('Bir hata oluştu.', 'error');
        });
}

function likeTopic(topicId) {
    fetch('/Topics/LikeTopic', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/x-www-form-urlencoded',
            'RequestVerificationToken': document.querySelector('input[name="__RequestVerificationToken"]')?.value || ''
        },
        body: `topicId=${topicId}`
    })
        .then(response => response.json())
        .then(data => {
            if (data.success) {
                // Beğeni sayısını güncelle
                const likeCountElement = document.getElementById(`topic-like-count-${topicId}`);
                if (likeCountElement) {
                    likeCountElement.textContent = data.likeCount;
                }

                // Buton rengini değiştir
                const likeButton = document.getElementById(`topic-like-btn-${topicId}`);
                if (likeButton) {
                    if (data.liked) {
                        likeButton.classList.remove('btn-outline-primary');
                        likeButton.classList.add('btn-primary');
                    } else {
                        likeButton.classList.remove('btn-primary');
                        likeButton.classList.add('btn-outline-primary');
                    }
                }

                // Başarı mesajı göster
                showMessage('Konu beğenisi güncellendi!', 'success');
            } else {
                showMessage(data.message || 'Beğeni işlemi başarısız oldu.', 'error');
            }
        })
        .catch(error => {
            console.error('Error:', error);
            showMessage('Bir hata oluştu.', 'error');
        });
}

// Yanıta yanıt yazma fonksiyonu
function replyToReply(replyId, username) {
    const form = document.getElementById('main-reply-form');
    if (!form) return;

    const parentReplyIdInput = document.querySelector('input[name="ParentReplyId"]');
    const contentTextarea = document.querySelector('textarea[name="Content"]');
    const cardHeader = form.querySelector('.card-header h5');
    const cancelButton = document.getElementById('cancel-reply');

    if (!parentReplyIdInput || !contentTextarea || !cardHeader || !cancelButton) return;

    // Form başlığını güncelle
    cardHeader.innerHTML = `<i class="bi bi-reply"></i> ${username} kullanıcısına yanıt yazıyorsunuz`;

    // ParentReplyId'yi set et
    parentReplyIdInput.value = replyId;

    // Placeholder'ı güncelle
    contentTextarea.placeholder = `${username} kullanıcısına yanıtınızı yazın...`;

    // İptal butonunu göster
    cancelButton.classList.remove('d-none');

    // Form'a scroll yap
    form.scrollIntoView({ behavior: 'smooth' });

    // Textarea'ya focus yap
    contentTextarea.focus();
}

// Yanıt iptal etme fonksiyonu
function cancelReply() {
    const form = document.getElementById('main-reply-form');
    if (!form) return;

    const parentReplyIdInput = document.querySelector('input[name="ParentReplyId"]');
    const contentTextarea = document.querySelector('textarea[name="Content"]');
    const cardHeader = form.querySelector('.card-header h5');
    const cancelButton = document.getElementById('cancel-reply');

    if (!parentReplyIdInput || !contentTextarea || !cardHeader || !cancelButton) return;

    // Form başlığını sıfırla
    cardHeader.innerHTML = '<i class="bi bi-reply"></i> Yanıt Yaz';

    // ParentReplyId'yi temizle
    parentReplyIdInput.value = '';

    // Placeholder'ı sıfırla
    contentTextarea.placeholder = 'Yanıtınızı yazın...';

    // İptal butonunu gizle
    cancelButton.classList.add('d-none');

    // Textarea'yı temizle
    contentTextarea.value = '';
}

// Mesaj gösterme fonksiyonu
function showMessage(message, type) {
    // Mevcut alert'leri temizle
    const existingAlerts = document.querySelectorAll('.temp-alert');
    existingAlerts.forEach(alert => alert.remove());

    // Yeni alert oluştur
    const alertDiv = document.createElement('div');
    alertDiv.className = `alert alert-${type === 'success' ? 'success' : 'danger'} alert-dismissible fade show temp-alert`;
    alertDiv.innerHTML = `
        <i class="bi bi-${type === 'success' ? 'check-circle' : 'exclamation-triangle'}"></i> ${message}
        <button type="button" class="btn-close" data-bs-dismiss="alert"></button>
    `;

    // Container'ın başına ekle
    const container = document.querySelector('.container');
    if (container) {
        container.insertBefore(alertDiv, container.firstChild);

        // 3 saniye sonra otomatik kapat
        setTimeout(() => {
            if (alertDiv.parentNode) {
                alertDiv.remove();
            }
        }, 3000);
    }
}