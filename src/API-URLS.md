# API Endpoint'leri

## 🌐 Base URL
```
http://localhost:8080
```

## 📚 Swagger UI (API Dokümantasyonu)
```
http://localhost:8080/swagger
```

## 🔐 Authentication (JWT Token almak için)
```
POST http://localhost:8080/api/identity/login
Content-Type: application/json

{
  "login": "admin@admin.com",
  "password": "A2m!nPassword",
  "rememberMe": false,
  "deviceDetails": "Browser"
}
```

## 🎮 Xtream API Gateway (IPTV Player'lar için)

### User Info (Authentication)
```
GET http://localhost:8080/api/player?username=test&password=test
```

### Live TV Categories
```
GET http://localhost:8080/api/player?username=test&password=test&action=get_live_categories
```

### Live TV Streams
```
GET http://localhost:8080/api/player?username=test&password=test&action=get_live_streams&category_id=3
```

### VOD Categories
```
GET http://localhost:8080/api/player?username=test&password=test&action=get_vod_categories
```

### VOD Streams
```
GET http://localhost:8080/api/player?username=test&password=test&action=get_vod_streams&category_id=10
```

### VOD Info
```
GET http://localhost:8080/api/player?username=test&password=test&action=get_vod_info&vod_id=1
```

### Series Categories
```
GET http://localhost:8080/api/player?username=test&password=test&action=get_series_categories
```

### Series
```
GET http://localhost:8080/api/player?username=test&password=test&action=get_series&category_id=20
```

### Series Info
```
GET http://localhost:8080/api/player?username=test&password=test&action=get_series_info&series_id=1
```

### EPG
```
GET http://localhost:8080/api/player?username=test&password=test&action=get_epg&category_id=3
```

## 🚀 Modern API (JWT Token Gerekli)

Önce token alın:
```bash
curl -X POST http://localhost:8080/api/identity/login \
  -H "Content-Type: application/json" \
  -d '{"login":"admin@admin.com","password":"A2m!nPassword","rememberMe":false,"deviceDetails":"test"}'
```

Sonra token ile istek yapın:
```
GET http://localhost:8080/api/v1/streaming/live/categories
Authorization: Bearer YOUR_TOKEN_HERE
```

### Modern API Endpoints:
- `GET /api/v1/streaming/live/categories`
- `GET /api/v1/streaming/live/streams?categoryId=3`
- `GET /api/v1/streaming/vod/categories`
- `GET /api/v1/streaming/vod/streams?categoryId=10`
- `GET /api/v1/streaming/vod/{vodId}`
- `GET /api/v1/streaming/series/categories`
- `GET /api/v1/streaming/series?categoryId=20`
- `GET /api/v1/streaming/series/{seriesId}`
- `GET /api/v1/streaming/epg?categoryId=3`
- `GET /api/v1/streaming/epg/short/{streamId}?limit=20`
- `GET /api/v1/streaming/epg/table/{streamId}`

## ⚠️ Önemli Notlar

1. **Root URL (`http://localhost:8080/`) çalışmaz** - Bu bir API, web sayfası değil
2. **Swagger UI kullanın**: `http://localhost:8080/swagger` - Tüm endpoint'leri test edebilirsiniz
3. **Xtream API için**: `username=test` ve `password=test` kullanın (seed data'dan)
4. **Modern API için**: Önce login yapıp JWT token alın

## 🧪 Test Komutları

```bash
# Swagger UI'yi aç
open http://localhost:8080/swagger

# Xtream API test
curl 'http://localhost:8080/api/player?username=test&password=test&action=get_live_categories'

# Login ve token al
curl -X POST http://localhost:8080/api/identity/login \
  -H "Content-Type: application/json" \
  -d '{"login":"admin@admin.com","password":"A2m!nPassword","rememberMe":false,"deviceDetails":"test"}'
```

