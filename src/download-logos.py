#!/usr/bin/env python3
"""
Geothermal.org sitesinden logo görsellerini indirme script'i
"""

import requests
import os
from bs4 import BeautifulSoup
from urllib.parse import urljoin, urlparse
import time

def download_logos():
    """Geothermal.org sitesinden logo görsellerini indirir"""
    
    # Ana URL
    base_url = "https://geothermal.org/our-organization/our-members"
    
    # Headers - gerçek bir tarayıcı gibi davranmak için
    headers = {
        'User-Agent': 'Mozilla/5.0 (Macintosh; Intel Mac OS X 10_15_7) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/120.0.0.0 Safari/537.36',
        'Accept': 'text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8',
        'Accept-Language': 'en-US,en;q=0.5',
        'Accept-Encoding': 'gzip, deflate',
        'Connection': 'keep-alive',
        'Upgrade-Insecure-Requests': '1',
    }
    
    try:
        print("🌐 Sayfa yükleniyor...")
        response = requests.get(base_url, headers=headers, timeout=30)
        response.raise_for_status()
        
        # HTML'i parse et
        soup = BeautifulSoup(response.content, 'html.parser')
        
        # Logo klasörü oluştur
        os.makedirs('geothermal_logos', exist_ok=True)
        
        # Tüm img tag'lerini bul
        images = soup.find_all('img')
        
        print(f"📸 {len(images)} adet görsel bulundu")
        
        downloaded_count = 0
        
        for i, img in enumerate(images):
            # src attribute'unu al
            src = img.get('src')
            if not src:
                continue
                
            # Tam URL'yi oluştur
            full_url = urljoin(base_url, src)
            
            # Dosya adını al
            filename = os.path.basename(urlparse(full_url).path)
            if not filename or '.' not in filename:
                filename = f"logo_{i}.jpg"
            
            # Alt text'i al (logo adı için)
            alt_text = img.get('alt', '').strip()
            if alt_text:
                # Dosya adını alt text'ten oluştur
                safe_name = "".join(c for c in alt_text if c.isalnum() or c in (' ', '-', '_')).rstrip()
                safe_name = safe_name.replace(' ', '_')
                if safe_name:
                    filename = f"{safe_name}.jpg"
            
            filepath = os.path.join('geothermal_logos', filename)
            
            try:
                print(f"⬇️ İndiriliyor: {filename}")
                
                # Görseli indir
                img_response = requests.get(full_url, headers=headers, timeout=30)
                img_response.raise_for_status()
                
                # Dosyayı kaydet
                with open(filepath, 'wb') as f:
                    f.write(img_response.content)
                
                downloaded_count += 1
                print(f"✅ Başarılı: {filename}")
                
                # Rate limiting - sunucuyu yormamak için
                time.sleep(0.5)
                
            except Exception as e:
                print(f"❌ Hata ({filename}): {str(e)}")
                continue
        
        print(f"\n🎉 İşlem tamamlandı!")
        print(f"📁 {downloaded_count} adet logo indirildi")
        print(f"📂 Klasör: {os.path.abspath('geothermal_logos')}")
        
    except Exception as e:
        print(f"❌ Genel hata: {str(e)}")

def download_specific_logos():
    """Bilinen logo URL'lerini indirir"""
    
    # Bilinen logo URL'leri
    logo_urls = [
        "https://geothermal.org/wp-content/uploads/2025/01/baker-hughes-logo-2025.png",
        "https://geothermal.org/wp-content/uploads/2025/01/calpine-geysers-logo.png",
        "https://geothermal.org/wp-content/uploads/2025/01/eavor-logo.png",
        "https://geothermal.org/wp-content/uploads/2025/01/exceed-geo-energy.png",
        "https://geothermal.org/wp-content/uploads/2025/01/quaise-logo-eev-2025.png",
        "https://geothermal.org/wp-content/uploads/2025/01/slb-logo.png",
        "https://geothermal.org/wp-content/uploads/2025/01/turboden-2025.png"
    ]
    
    headers = {
        'User-Agent': 'Mozilla/5.0 (Macintosh; Intel Mac OS X 10_15_7) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/120.0.0.0 Safari/537.36',
        'Referer': 'https://geothermal.org/our-organization/our-members'
    }
    
    os.makedirs('geothermal_logos', exist_ok=True)
    
    for url in logo_urls:
        try:
            filename = os.path.basename(urlparse(url).path)
            filepath = os.path.join('geothermal_logos', filename)
            
            print(f"⬇️ İndiriliyor: {filename}")
            
            response = requests.get(url, headers=headers, timeout=30)
            response.raise_for_status()
            
            with open(filepath, 'wb') as f:
                f.write(response.content)
            
            print(f"✅ Başarılı: {filename}")
            time.sleep(0.5)
            
        except Exception as e:
            print(f"❌ Hata ({filename}): {str(e)}")

if __name__ == "__main__":
    print("🚀 Geothermal.org Logo İndirici")
    print("=" * 40)
    
    choice = input("Hangi yöntemi kullanmak istiyorsunuz?\n1. Otomatik tarama\n2. Bilinen URL'ler\nSeçiminiz (1/2): ")
    
    if choice == "1":
        download_logos()
    elif choice == "2":
        download_specific_logos()
    else:
        print("❌ Geçersiz seçim!") 