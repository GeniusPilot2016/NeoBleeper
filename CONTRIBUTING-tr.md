# 🤝 NeoBleeper'a Katkıda Bulunma

Öncelikle, NeoBleeper'a katkıda bulunmayı düşündüğünüz için teşekkür ederiz! Katkılarınız bu projenin başarısı için çok önemli. İster bir hata bildirin, ister bir özellik önerin, ister dokümantasyonu iyileştirin, ister eski BMM veya NBPML dosyası yükleyin, ister kod gönderin, katılımınız bizim için çok değerli.

## 📑 İçindekiler
1. [Davranış Kuralları](#davranış-kuralları)
2. [Nasıl Katkıda Bulunabilirim?](#nasıl-katkıda-bulabilirim)
- [Hata Raporları](#hata-raporları)
- [Özellik İstekleri](#özellik-istekleri)
- [Kod Katkıları](#kod-katkıları)
- [Belgeler](#belgeler)
- [BMM ve NBPML Dosya Katkıları](#bmm-ve-nbpml-dosya-katkıları)
3. [Çekme İsteği Süreci](#çekme-isteği-süreci)
4. [Stil Kılavuzları](#stil-kılavuzlar)
- [Kod Stili](#kod-stili)
- [C#'ye Özgü Notlar](#c-sharp-özgü-notlar)
5. [Topluluk Desteği](#topluluk-desteği)

## 🌟 Davranış Kuralları
Bu projeye katılarak, Davranış Kuralları'na uymayı kabul etmiş olursunuz. Lütfen topluluktaki diğer kişilere karşı saygılı ve düşünceli olun. Ayrıntılar için `CODE_OF_CONDUCT.md` dosyasına bakın.

## 🤝🙋‍♂️ Nasıl Katkıda Bulunabilirim?

### 🪲 Hata Raporları
NeoBleeper'da bir hata bulduysanız, lütfen bir sorun oluşturun ve aşağıdaki ayrıntıları ekleyin:
- Açık ve açıklayıcı bir başlık.
- NeoBleeper sürümü veya varsa commit hash'i.
- Sorunu yeniden oluşturma adımları veya bir kod parçacığı.
- Beklenen ve gerçekleşen davranış.
- Ekran görüntüleri veya çökme günlükleri dahil olmak üzere diğer ilgili ayrıntılar.

### 💭 Özellik İstekleri
Fikirlerinizi bekliyoruz! Bir özellik talep etmek için:
1. Başka birinin daha önce talep edip etmediğini görmek için sorunları kontrol edin. 2. Değilse, yeni bir sorun açın ve aşağıdakileri içeren ayrıntılı bir açıklama paylaşın:
- Talebin arka planı.
- Neden değerli olduğu.
- Potansiyel etkiler, riskler veya hususlar.

### 👩‍💻 Kod Katkıları
1. Depoyu çatallandırın ve `main` dışında yeni bir dal oluşturun. Dalınıza `feature/add-tune-filter` gibi açıklayıcı bir ad verin.
2. Depo klasörünü Visual Studio'da açın:
- Gerekli iş yükleriyle (örneğin, NeoBleeper için ".NET masaüstü geliştirme") [Visual Studio](https://visualstudio.microsoft.com/) yüklü olduğundan emin olun.
- Depo çatalınızı yerel makinenize kopyalayın (Visual Studio'nun entegre Git araçlarını veya Git CLI'sini kullanabilirsiniz).
- Klonlandıktan sonra, çözüm (`.sln`) dosyasını Visual Studio'da açın. 3. NuGet Paketlerini Yükleyin:
- Üst çubuktaki `NuGet Paketlerini Geri Yükle` seçeneğine tıklayarak veya terminalden `dotnet restore` komutunu çalıştırarak gerekli bağımlılıkları geri yükleyin.
4. Değişikliklerinizi ekleyin:
- Etkili bir şekilde katkıda bulunmak için IntelliSense, hata ayıklama ve kod biçimlendirme gibi Visual Studio özelliklerini kullanın.
- Uygun testlerin eklendiğinden ve mevcut tüm testlerin geçtiğinden emin olun.
- Kodunuzun stil kılavuzuna uygun olduğundan emin olun.
5. Hakkımızda Sayfasına adınızı veya takma adınızı ekleyin:
- `about_neobleeper.cs` dosyasını açın ve `listView1` bileşenini bulun.
- Visual Studio tasarımcısında `listView1` bileşenini seçin.
- Açılır menüyü açmak için bileşenin sağ üst köşesindeki küçük oka tıklayın.
- ListView öğeleri koleksiyonu düzenleyicisini açmak için `Öğeleri Düzenle`yi seçin.
- Yeni bir `ListViewItem` ekleyin:
- `Metin` özelliğine adınızı veya takma adınızı yazın. - Katkılarınız/görevleriniz için:
- **Alt Öğeler** özelliğini bulun.
- `(Koleksiyon)` alanının sağındaki üç noktaya (`...`) tıklayın.
- Görevlerinizin kısa bir açıklamasıyla **Alt Öğe**'yi ekleyin veya düzenleyin.
- Adınızı zaten eklediyseniz, değişikliklerinizi onaylamadan önce Alt Öğe'yi düzenleyin veya mevcut girdinizi güncelleyin.
6. Kodunuzu test edin:
- Testleri Visual Studio'nun Test Gezgini'ni kullanarak çalıştırın.
- Başarısız olan testleri düzeltin ve değişikliklerinizi doğrulayın.
7. Değişikliklerinizi açık ve öz mesajlarla onaylayın.
- Değişikliklerinizi hazırlamak ve onaylamak için Visual Studio'nun entegre Git araçlarını kullanın.
8. Dalınızı gönderin ve depoda bir çekme isteği açın.
9. Gözden geçirenlerle çalışmaya ve gerektiğinde düzeltme yapmaya hazır olun.

### 🧾 Belgeler
Belgelerimizi iyileştirmek, katkıda bulunmanın en kolay yollarından biridir! Örnek eklemekten veya güncellemekten, bölümleri açıklığa kavuşturmaktan veya genel okunabilirliği artırmaktan çekinmeyin.

### 🎼 BMM ve NBPML Dosya Katkıları
NeoBleeper, eski BMM (Bleeper Music Maker) ve NBPML (NeoBleeper Proje İşaretleme Dili) dosyalarını destekler. Bu dosya türlerine katkıda bulunuyor veya bu dosyalarla çalışıyorsanız, aşağıdakileri sağlayınız:
- BMM dosyalarının doğru şekilde ayrıştırıldığını ve NeoBleeper'da beklendiği gibi işlendiğini doğrulayınız.
- Hem eski formatlarla hem de mevcut uygulamayla uyumluluğu test ediniz.
- NBPML dosyaları için en son NeoBleeper Proje İşaretleme Dili özelliklerine uyunuz.

Bu dosya formatlarına özgü herhangi bir sorunla karşılaşırsanız, lütfen "Hata Raporları" bölümündeki yönergeleri izleyin. BMM ve NBPML dosyaları için gelişmiş destek özelliği talepleriniz de memnuniyetle karşılanır!

## ⬇️ Çekme İsteği Süreci
Tüm gönderimler çekme istekleri aracılığıyla yapılmalıdır. Süreç şu şekildedir:
1. Çekme isteği şablonunu doldurun.
2. Çekme isteğinizin mevcut olanları kopyalamadığından emin olunuz.
3. Değişikliklerinizin ayrıntılarını açıklamaya ekleyin ve mümkünse ilgili sorunlara atıfta bulununuz.
4. İncelemecilerden gelen tüm yorumları veya talep edilen değişiklikleri ele alınız.
5. Çekme istekleri, testler ve kod kalitesi kontrolleri de dahil olmak üzere tüm CI/CD kontrollerinden geçmelidir.

## 📖 Stil Kılavuzları
### ✨ Kod Stili
[.NET Kodlama Kuralları](https://learn.microsoft.com/en-us/dotnet/csharp/fundamentals/coding-style/coding-conventions)'nı izleyin. Önemli noktalar şunlardır:
- Genel alanlar yerine otomatik özellikleri tercih ediniz.
- Tür belli olduğunda yerel değişkenler için `var` kullanınız.
- Sihirli dizelerden ve sayılardan kaçınınız. Sabitler veya enumlar kullanınız.

### 📒 C#'a Özel Notlar
- `{` işaretini önceki kodla aynı satıra yerleştiriniz.
- Sınıf ve metot adları için PascalCase, yerel değişkenler için camelCase kullanınız.
- [Microsoft Adlandırma Yönergeleri](https://learn.microsoft.com/en-us/dotnet/standard/design-guidelines/naming-guidelines)'ni izleyiniz.

## 👨‍👩‍👧‍👦 Topluluk Desteği
Herhangi bir sorunuz varsa, bir GitHub Tartışması açabilir veya sorunlar bölümünden bize ulaşabilirsiniz. Herkesi bilgilerini paylaşmaya ve diğer işbirlikçilere destek olmaya teşvik ediyoruz.

NeoBleeper'a katkıda bulunduğunuz ve inanılmaz bir şey inşa etmenize yardımcı olduğunuz için teşekkür ederiz!
