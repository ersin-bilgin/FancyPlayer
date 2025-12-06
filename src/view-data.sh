#!/bin/bash
# PostgreSQL verilerini görüntülemek için yardımcı script

echo "=== PostgreSQL Verilerini Görüntüleme ==="
echo ""

# Interactive psql bağlantısı
echo "1. Interactive psql bağlantısı için:"
echo "   docker compose exec postgresql psql -U lightnap_user -d LightNap"
echo ""

# Örnek sorgular
echo "2. Örnek sorgular:"
echo ""
echo "   # Tüm tabloları listele:"
echo "   docker compose exec -T postgresql psql -U lightnap_user -d LightNap -c '\\dt'"
echo ""
echo "   # Live Categories:"
echo "   docker compose exec -T postgresql psql -U lightnap_user -d LightNap -c 'SELECT * FROM \"LiveCategories\" ORDER BY \"SortOrder\";'"
echo ""
echo "   # Live Streams:"
echo "   docker compose exec -T postgresql psql -U lightnap_user -d LightNap -c 'SELECT ls.\"Id\", ls.\"StreamName\", lc.\"Name\" as category FROM \"LiveStreams\" ls LEFT JOIN \"LiveCategories\" lc ON ls.\"CategoryId\" = lc.\"Id\";'"
echo ""
echo "   # VOD Movies:"
echo "   docker compose exec -T postgresql psql -U lightnap_user -d LightNap -c 'SELECT vm.\"Id\", vm.\"Title\", vc.\"Name\" as category, vm.\"Rating\" FROM \"VodMovies\" vm LEFT JOIN \"VodCategories\" vc ON vm.\"CategoryId\" = vc.\"Id\";'"
echo ""
echo "   # Series:"
echo "   docker compose exec -T postgresql psql -U lightnap_user -d LightNap -c 'SELECT s.\"Id\", s.\"Title\", sc.\"Name\" as category FROM \"Series\" s LEFT JOIN \"SeriesCategories\" sc ON s.\"CategoryId\" = sc.\"Id\";'"
echo ""
echo "   # Xtream Users:"
echo "   docker compose exec -T postgresql psql -U lightnap_user -d LightNap -c 'SELECT \"Id\", \"Username\", \"FullName\", \"Status\", \"MaxConnections\" FROM \"XtreamUsers\";'"
echo ""
echo "   # EPG Programmes:"
echo "   docker compose exec -T postgresql psql -U lightnap_user -d LightNap -c 'SELECT ep.\"Title\", ep.\"StartTime\", ep.\"EndTime\", ec.\"EpgId\" FROM \"EpgProgrammes\" ep JOIN \"EpgChannels\" ec ON ep.\"EpgChannelId\" = ec.\"Id\";'"
echo ""

