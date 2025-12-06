# Drupal Grid Sistemi - Görsel Erişim Çözümleri

## 🔍 Sorun Analizi

Drupal üzerinde grid sistemi içinde görsel ekleme sırasında yaşanan erişim sorunları:

1. **CORS (Cross-Origin Resource Sharing) Kısıtlamaları**
2. **Drupal File System Erişim Sorunları**
3. **Grid Modülü Konfigürasyon Problemleri**
4. **Görsel URL'lerinin Yanlış Oluşturulması**

## 🛠️ Çözüm Yöntemleri

### Yöntem 1: Drupal File System Konfigürasyonu

```php
// settings.php dosyasında
$settings['file_public_path'] = 'sites/default/files';
$settings['file_private_path'] = 'sites/default/files/private';

// CORS ayarları
$settings['cors.config'] = [
  'allowedOrigins' => ['*'],
  'allowedHeaders' => ['*'],
  'allowedMethods' => ['GET', 'POST', 'PUT', 'DELETE'],
  'exposedHeaders' => [],
  'maxAge' => 1000,
  'supportsCredentials' => FALSE,
];
```

### Yöntem 2: Grid Modülü için Custom CSS

```css
/* Grid içindeki görseller için CSS */
.grid-item img {
  max-width: 100%;
  height: auto;
  display: block;
  object-fit: cover;
}

/* Görsel yükleme durumu için */
.grid-item img[src*="placeholder"] {
  background: #f0f0f0;
  min-height: 200px;
}
```

### Yöntem 3: Custom Module ile Görsel Erişimi

```php
<?php
/**
 * @file
 * Custom module for grid image access.
 */

/**
 * Implements hook_theme().
 */
function custom_grid_images_theme() {
  return [
    'grid_image_item' => [
      'variables' => [
        'image_url' => NULL,
        'alt_text' => NULL,
        'title' => NULL,
      ],
    ],
  ];
}

/**
 * Custom function to get image URL with proper access.
 */
function custom_grid_images_get_image_url($fid) {
  $file = \Drupal\file\Entity\File::load($fid);
  if ($file) {
    return file_create_url($file->getFileUri());
  }
  return '';
}

/**
 * Implements hook_preprocess_HOOK().
 */
function custom_grid_images_preprocess_grid_image_item(&$variables) {
  // Ensure proper image URL generation
  if (isset($variables['image_url'])) {
    $variables['image_url'] = file_create_url($variables['image_url']);
  }
}
```

### Yöntem 4: JavaScript ile Görsel Yükleme Kontrolü

```javascript
// Grid görsellerini kontrol eden JavaScript
document.addEventListener('DOMContentLoaded', function() {
  const gridImages = document.querySelectorAll('.grid-item img');
  
  gridImages.forEach(function(img) {
    // Görsel yükleme hatası kontrolü
    img.addEventListener('error', function() {
      console.log('Görsel yüklenemedi:', img.src);
      
      // Placeholder görsel göster
      img.src = '/sites/default/files/placeholder-image.png';
      img.alt = 'Görsel yüklenemedi';
      
      // Hata durumunu logla
      fetch('/admin/reports/grid-image-errors', {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
        },
        body: JSON.stringify({
          failed_url: img.src,
          timestamp: new Date().toISOString()
        })
      });
    });
    
    // Görsel yükleme başarı kontrolü
    img.addEventListener('load', function() {
      console.log('Görsel başarıyla yüklendi:', img.src);
    });
  });
});
```

### Yöntem 5: Drupal Views Konfigürasyonu

```yaml
# views.grid_images.yml
display:
  id: grid_images
  display_title: 'Grid Images'
  display_plugin: grid
  position: 1
  display_options:
    style:
      type: grid
      options:
        columns: 3
        alignment: center
    fields:
      field_image:
        id: field_image
        table: node__field_image
        field: field_image
        relationship: none
        group_type: group
        admin_label: ''
        entity_type: node
        entity_field: field_image
        plugin_id: field
        label: ''
        exclude: false
        alter:
          alter_text: false
          text: ''
          make_link: false
          path: ''
          absolute: false
          external: false
          replace_spaces: false
          path_case: none
          trim_whitespace: false
          alt: ''
          rel: ''
          link_class: ''
          prefix: ''
          suffix: ''
          target: ''
          nl2br: false
          max_length: 0
          word_boundary: true
          ellipsis: true
          more_link: false
          more_link_text: ''
          more_link_path: ''
          strip_tags: false
          trim: false
          preserve_tags: ''
          html: false
        element_type: ''
        element_class: ''
        element_label_type: ''
        element_label_class: ''
        element_label_colon: true
        element_wrapper_type: ''
        element_wrapper_class: ''
        element_default_classes: true
        empty: ''
        hide_empty: false
        empty_zero: false
        hide_alter_empty: true
        click_sort_column: target_id
        type: image
        settings:
          image_style: grid_thumbnail
          image_link: ''
        group_column: target_id
        group_columns: ''
        group_rows: true
        delta_limit: 0
        delta_offset: 0
        delta_reversed: false
        delta_first_last: false
        multi_type: separator
        separator: ', '
        field_api_classes: false
```

## 🔧 Debug Yöntemleri

### 1. Drupal Debug Modu
```bash
# settings.php'de debug modunu aktifleştir
$config['system.logging']['error_level'] = 'verbose';
```

### 2. Browser Developer Tools
```javascript
// Console'da görsel URL'lerini kontrol et
document.querySelectorAll('img').forEach(img => {
  console.log('Image URL:', img.src);
  console.log('Image Alt:', img.alt);
});
```

### 3. Drupal Log Kontrolü
```bash
# Drupal log dosyasını kontrol et
tail -f /path/to/drupal/sites/default/files/php.log
```

## 📋 Kontrol Listesi

- [ ] Drupal File System izinleri kontrol edildi
- [ ] CORS ayarları yapılandırıldı
- [ ] Grid modülü güncel versiyona güncellendi
- [ ] Görsel URL'leri doğru oluşturuluyor
- [ ] Placeholder görseller hazırlandı
- [ ] JavaScript hata yakalama eklendi
- [ ] Drupal cache temizlendi

## 🚀 Hızlı Çözüm

Eğer acil çözüm gerekiyorsa:

1. **Drupal Cache Temizleme:**
   ```bash
   drush cr
   ```

2. **File System İzinleri:**
   ```bash
   chmod -R 755 sites/default/files
   ```

3. **Temporary Çözüm:**
   ```php
   // Görsel URL'lerini manuel olarak düzelt
   $image_url = str_replace('private://', 'public://', $image_url);
   ```

Bu çözümler Drupal grid sistemi üzerinde görsel erişim sorunlarını çözmeye yardımcı olacaktır. 