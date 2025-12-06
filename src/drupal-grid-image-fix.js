/**
 * Drupal Grid Image Access Fix
 * Bu script Drupal grid sistemi üzerinde görsel erişim sorunlarını çözer
 */

(function ($, Drupal) {
  'use strict';

  Drupal.behaviors.gridImageAccessFix = {
    attach: function (context, settings) {
      
      // Grid içindeki tüm görselleri bul
      const gridImages = document.querySelectorAll('.grid-item img, .views-grid img, .field--type-image img');
      
      gridImages.forEach(function(img) {
        
        // Görsel yükleme hatası kontrolü
        img.addEventListener('error', function() {
          console.warn('Grid görsel yüklenemedi:', img.src);
          
          // Orijinal URL'yi kaydet
          const originalSrc = img.src;
          
          // URL'yi düzeltmeye çalış
          let fixedSrc = originalSrc;
          
          // Private file path'ini public'e çevir
          if (fixedSrc.includes('private://')) {
            fixedSrc = fixedSrc.replace('private://', 'public://');
          }
          
          // File system path'ini URL'ye çevir
          if (fixedSrc.includes('sites/default/files/')) {
            fixedSrc = fixedSrc.replace('sites/default/files/', '/sites/default/files/');
          }
          
          // Eğer hala aynı URL ise, placeholder göster
          if (fixedSrc === originalSrc) {
            img.src = '/sites/default/files/placeholder-image.png';
            img.alt = 'Görsel yüklenemedi';
            img.style.border = '2px dashed #ccc';
            img.style.backgroundColor = '#f9f9f9';
          } else {
            // Düzeltilmiş URL'yi dene
            img.src = fixedSrc;
          }
          
          // Hata durumunu logla
          logImageError(originalSrc, fixedSrc);
        });
        
        // Görsel başarıyla yüklendiğinde
        img.addEventListener('load', function() {
          console.log('Grid görsel başarıyla yüklendi:', img.src);
          img.style.border = 'none';
          img.style.backgroundColor = 'transparent';
        });
        
        // Görsel yüklenirken loading göster
        img.addEventListener('loadstart', function() {
          img.style.opacity = '0.5';
          img.style.transition = 'opacity 0.3s';
        });
        
        img.addEventListener('loadend', function() {
          img.style.opacity = '1';
        });
      });
      
      // Grid container'ı için CSS düzeltmeleri
      const gridContainers = document.querySelectorAll('.grid, .views-grid, .field--type-image');
      gridContainers.forEach(function(container) {
        container.style.display = 'grid';
        container.style.gridTemplateColumns = 'repeat(auto-fit, minmax(200px, 1fr))';
        container.style.gap = '20px';
        container.style.padding = '20px';
      });
    }
  };
  
  /**
   * Görsel hata durumunu loglar
   */
  function logImageError(originalSrc, fixedSrc) {
    const errorData = {
      original_url: originalSrc,
      fixed_url: fixedSrc,
      timestamp: new Date().toISOString(),
      user_agent: navigator.userAgent,
      page_url: window.location.href
    };
    
    // Console'a log
    console.error('Grid Image Error:', errorData);
    
    // Server'a hata raporu gönder (opsiyonel)
    if (Drupal.settings.gridImageFix && Drupal.settings.gridImageFix.logErrors) {
      fetch('/admin/reports/grid-image-errors', {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
          'X-CSRF-Token': Drupal.settings.gridImageFix.csrfToken
        },
        body: JSON.stringify(errorData)
      }).catch(function(error) {
        console.warn('Hata raporu gönderilemedi:', error);
      });
    }
  }
  
  /**
   * Görsel URL'lerini düzeltmek için utility fonksiyon
   */
  Drupal.gridImageFix = {
    fixImageUrl: function(url) {
      if (!url) return '';
      
      let fixedUrl = url;
      
      // Private file path'ini public'e çevir
      if (fixedUrl.includes('private://')) {
        fixedUrl = fixedUrl.replace('private://', 'public://');
      }
      
      // File system path'ini URL'ye çevir
      if (fixedUrl.includes('sites/default/files/')) {
        fixedUrl = fixedUrl.replace('sites/default/files/', '/sites/default/files/');
      }
      
      // Relative path'i absolute path'e çevir
      if (fixedUrl.startsWith('/sites/')) {
        fixedUrl = window.location.origin + fixedUrl;
      }
      
      return fixedUrl;
    },
    
    createPlaceholder: function(width, height) {
      const canvas = document.createElement('canvas');
      canvas.width = width || 200;
      canvas.height = height || 200;
      const ctx = canvas.getContext('2d');
      
      // Placeholder arka plan
      ctx.fillStyle = '#f0f0f0';
      ctx.fillRect(0, 0, canvas.width, canvas.height);
      
      // Placeholder metni
      ctx.fillStyle = '#999';
      ctx.font = '14px Arial';
      ctx.textAlign = 'center';
      ctx.fillText('Görsel Yüklenemedi', canvas.width / 2, canvas.height / 2);
      
      return canvas.toDataURL();
    }
  };
  
  // Sayfa yüklendiğinde çalıştır
  $(document).ready(function() {
    // Grid görsellerini yeniden kontrol et
    setTimeout(function() {
      Drupal.behaviors.gridImageAccessFix.attach(document, Drupal.settings);
    }, 1000);
  });
  
})(jQuery, Drupal); 