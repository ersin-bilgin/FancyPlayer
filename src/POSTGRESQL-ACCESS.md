# PostgreSQL'e Erişim Rehberi

## ⚠️ ÖNEMLİ: Port 5432 Web Sunucusu Değil!

**Port 5432** = PostgreSQL Veritabanı Sunucusu (HTTP değil, SQL protokolü)
**Port 8080** = Web API Sunucusu (HTTP)

Tarayıcıda `http://localhost:5432/` açılmaz çünkü PostgreSQL bir web sunucusu değil!

## 🔧 PostgreSQL'e Erişim Yöntemleri

### 1. Docker Üzerinden (En Kolay) ✅

```bash
cd src
docker compose exec postgresql psql -U lightnap_user -d LightNap
```

psql içinde:
```sql
-- Tüm tabloları listele
\dt

-- Verileri görüntüle
SELECT * FROM "LiveCategories";
SELECT * FROM "LiveStreams";
SELECT * FROM "VodMovies";

-- Çıkış
\q
```

### 2. GUI Tool Kullanın (Önerilen) 🎯

#### Option A: pgAdmin (Web Tabanlı)

**pgAdmin'i Docker ile çalıştırın:**

`docker-compose.yml` dosyasına ekleyin:
```yaml
pgadmin:
  image: dpage/pgadmin4:latest
  container_name: lightnap-pgadmin
  environment:
    PGADMIN_DEFAULT_EMAIL: admin@admin.com
    PGADMIN_DEFAULT_PASSWORD: admin
  ports:
    - "5050:80"
  networks:
    - lightnap-network
```

Sonra:
```bash
docker compose up -d pgadmin
```

Tarayıcıda açın: **http://localhost:5050**

Bağlantı bilgileri:
- Host: `postgresql` (Docker network içinde) veya `host.docker.internal` (local'den)
- Port: `5432`
- Database: `LightNap`
- Username: `lightnap_user`
- Password: `LightNap123!`

#### Option B: TablePlus (macOS - En İyi Seçenek)

1. İndir: https://tableplus.com/
2. Yeni bağlantı → PostgreSQL
3. Bilgiler:
   - **Name**: `LightNap Local`
   - **Host**: `localhost`
   - **Port**: `5432`
   - **User**: `lightnap_user`
   - **Password**: `LightNap123!`
   - **Database**: `LightNap`

#### Option C: DBeaver (Ücretsiz, Cross-Platform)

1. İndir: https://dbeaver.io/
2. Yeni Database Connection → PostgreSQL
3. Aynı bilgileri girin

### 3. Local psql Kurulumu (macOS)

```bash
# Homebrew ile kurulum
brew install postgresql@16

# veya sadece client
brew install libpq
echo 'export PATH="/opt/homebrew/opt/libpq/bin:$PATH"' >> ~/.zshrc
source ~/.zshrc

# Bağlan
PGPASSWORD=LightNap123! psql -h localhost -p 5432 -U lightnap_user -d LightNap
```

## 📊 Bağlantı Bilgileri

```
Host: localhost (veya 127.0.0.1)
Port: 5432
Database: LightNap
Username: lightnap_user
Password: LightNap123!
```

## 🔗 Connection String

GUI tool'lar için:
```
postgresql://lightnap_user:LightNap123!@localhost:5432/LightNap
```

## 🧪 Hızlı Test

```bash
# Docker üzerinden test
docker compose exec postgresql psql -U lightnap_user -d LightNap -c "SELECT COUNT(*) FROM \"LiveCategories\";"

# Verileri görüntüle
docker compose exec postgresql psql -U lightnap_user -d LightNap -c "SELECT * FROM \"LiveCategories\" ORDER BY \"SortOrder\";"
```

## 🌐 Web Arayüzü İçin

Eğer PostgreSQL için web arayüzü istiyorsanız:

1. **pgAdmin** (Docker ile - yukarıdaki yöntem)
2. **Adminer** (Hafif alternatif)
3. **TablePlus** (Desktop app - önerilen)

## ⚡ Hızlı Başlangıç

En kolay yöntem: **TablePlus** kurun ve bağlanın!



