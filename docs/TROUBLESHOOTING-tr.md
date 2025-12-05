# NeoBleeper Sorun Giderme Rehberi

Bu kılavuz, NeoBleeper programını kullanırken karşılaşılan yaygın sorunlara, özellikle sistem hoparlörü davranışı, ses çıkışı, donanım uyumluluğu ve sürekli sistem bip sesi ile ilgili sorunlara yönelik çözümler sağlar.

---

## 1. Çökme veya Zorla Kapatma Sonrasında Sistem Hoparlöründe Ses Takılması

**Sorun:**
NeoBleeper, sistem (PC) hoparlöründen ses çalarken çökerse veya zorla kapatılırsa, ses "takılabilir" ve bu da sürekli bip sesi veya vızıltıya neden olabilir.

**Neden Olur?:**
Sistem hoparlörü düşük donanım/yazılım seviyesinde kontrol edilir. Uygulama çıkışta hoparlörü düzgün bir şekilde serbest bırakmazsa veya sıfırlanmazsa ses devam edebilir.

**Çözümler:**
- **NeoBleeper Bip Sesi Durdurucu yardımcı programını kullanın (64 bit sürümü için):**
  NeoBleeper, program klasöründe "NeoBleeper Bip Sesi Durdurucu" adlı bir araç ile birlikte gelir.

  ![image4](https://github.com/user-attachments/assets/b36256ad-916d-42ab-83ea-3271dd897ce1)
  
  - Bu aracı çalıştırın ve sistem hoparlöründen gelen takılıp kalmış bip sesini durdurmak için **Bip Sesini Durdur** düğmesine basınız.
  - Bu aracı yalnızca bir çökme veya zorla çıkıştan sonra bip sesi devam ettiğinde kullanınız.

  #### Bip Durdurucu Mesajları ve Anlamları

  Bip Sesi Durdurucu yardımcı programını kullandığınızda aşağıdaki mesajları görebilirsiniz:

  ![image1](https://github.com/user-attachments/assets/54de81c0-1ff0-4eb3-ace7-652110c2e5e9)
    
    **Sistem hoparlörü bip sesi çıkarmıyor veya sistem hoparlörü farklı bir şekilde bip sesi çıkarıyor. Herhangi bir işlem yapılmadı.**  
    Bu mesaj, yardımcı program sistem hoparlörünü kontrol ettiğinde ve hoparlörün bip sesi çıkarmadığını veya araç tarafından kontrol edilemeyen bir şekilde bip sesi çıkardığını tespit ettiğinde görüntülenir. Bu durumda, Bip Durdurucu başka bir işlem yapmaz.
    - *İpucu:* Hala sürekli bir bip sesi duyuyorsanız, bilgisayarınızı yeniden başlatmayı deneyin.

  ![image2](https://github.com/user-attachments/assets/6b398155-f3f5-46dd-b05c-0d040c287dc5)
    
    **Bip sesi başarıyla durduruldu!**  
    Bu ileti, Bip Sesi DUrdurucu yardımcı programının takılı kalmış bir bip sesi algıladığını ve bunu başarıyla durdurabildiğini onaylar. Başka bir işlem yapmanıza gerek yoktur.
  
  ![image3](https://github.com/user-attachments/assets/b43f46ca-a1e9-44f3-a835-833eab73d0dc)
  
    **Sistem hoparlörü çıkışı mevcut değil veya standart dışı bir sistem hoparlörü çıkışı mevcut. Bip sesi durdurucu, kararsızlıklara veya istenmeyen davranışlara neden olabilir. Devam etmek istiyor musunuz?**  
    Bu mesaj, Bip Sesi Durdurucu yardımcı programı başlatıldığında ve sisteminizde standart bir sistem (PC) hoparlörü olmadığı veya sistem hoparlörü çıkışının "standart dışı" olduğu tespit edildiğinde görüntülenir. Bu durumda, yardımcı program sizi Bip Sesi Durdurucu'yu kullanmaya çalışmanın beklendiği gibi çalışmayabileceği ve beklenmedik davranışlara veya dengesizliğe neden olabileceği konusunda uyarır.

    Devam ederseniz, araç bip sesini durdurmaya çalışır, ancak donanımınız desteklenmiyorsa veya standart dışı ise bu işlem etkisiz olabilir veya yan etkilere neden olabilir.
    
    Devam etmemeyi seçerseniz, araç herhangi bir değişiklik yapmadan kapanır.
    - *İpucu:* Bu mesajı alırsanız, bilgisayarınızda uyumlu bir sistem hoparlörü olmadığı veya çıkışının güvenilir bir şekilde kontrol edilemediği anlamına gelir. Duyduğunuz herhangi bir bip sesi veya vızıltı muhtemelen başka bir ses aygıtından (ana hoparlörleriniz veya kulaklıklarınız gibi) geliyordur. Ses sorunlarını gidermek için standart ses aygıtı ayarlarınızı kullanın ve istenmeyen ses üreten uygulamaları kapatın. Sorun devam ederse, bilgisayarınızı yeniden başlatmayı veya cihazınızın ses ayarlarını kontrol etmeyi deneyin.
      
- **Bilgisayarınızı yeniden başlatın:**
  Bip Sesi Durdurucu sorunu çözmezse, sistem yeniden başlatıldığında hoparlör donanımı sıfırlanacaktır.

- **Önlem:**
  NeoBleeper'ı her zaman normal şekilde kapatın. Ses çalarken Görev Yöneticisi veya benzeri araçlar aracılığıyla zorla kapatmaktan kaçının.
---

## 2. Sistem Hoparlörü Algılama ve Uyumluluk

NeoBleeper, sisteminizde standart bir PC hoparlörü çıkışı olup olmadığını kontrol etmek için algılama mantığının yanı sıra "gizli" sistem hoparlörü çıkışları (örneğin PNP0800 kimliğini kullanmayanlar) için destek içerir. Donanımınız standart veya gizli bir sistem hoparlörünü desteklemiyorsa veya çıkış standart dışı ve kullanılamıyorsa, uyarı mesajları görebilir veya bip sesleri için normal ses cihazınıza güvenmek zorunda kalabilirsiniz. Ancak, son sürümlerden itibaren NeoBleeper, standart bir hoparlör eksik olduğunda yalnızca ses cihazını kullanmanızı zorunlu kılmıyor; artık varsa gizli/PNP0800 olmayan sistem hoparlör çıkışlarının kullanımına izin veriyor.

### Örnek Uyarı (Resim 1):

![resim1](https://github.com/user-attachments/assets/8df2c1a5-a442-46af-bcef-71610aee3706)

> **Açıklama:**
> Bilgisayarınızın anakartında standart bir sistem hoparlörü çıkışı yok veya çıkış standart dışı. NeoBleeper, PNP0800 olarak tanımlanmayan "gizli" sistem hoparlörü çıkışlarını algılayıp kullanıma sunmaya çalışacaktır. Böyle bir çıkış mevcutsa, bu uyarı görünse bile sistem hoparlörünü kullanabilirsiniz. Aksi takdirde, NeoBleeper normal ses aygıtınıza (hoparlör veya kulaklık gibi) geri dönecektir.

### Ayarlar İletişim Kutuları (Resim 2 ve 3):

![image2](https://github.com/user-attachments/assets/c5f72889-76c5-4088-80b3-d9a27413ee73)

![image3](https://github.com/user-attachments/assets/b3db6fe9-04ae-4888-be99-2c6a2b156cfa)


- **"Sistem Hoparlörünü Test Et" düğmesinin kullanılabilirliği:**
  NeoBleeper, gizli veya PNP0800 olmayan çıkışlar da dahil olmak üzere kullanılabilir herhangi bir sistem hoparlörü çıkışı algılarsa bu seçenek etkinleştirilir.

- **"Bip sesi oluşturmak için ses cihazını kullan" ayarı:**
  Artık gizli veya standart dışı bir sistem hoparlörü çıkışı algılanırsa bu özelliği devre dışı bırakabilirsiniz.

#### "Standart dışı sistem hoparlörü çıkışı" ne anlama geliyor?
Bazı modern bilgisayarlarda, dizüstü bilgisayarlarda veya sanal makinelerde gerçek bir PC hoparlörü bulunmaz veya sinyal yönlendirmesi standart dışıdır. NeoBleeper artık bu tür gizli sistem hoparlörü çıkışlarını (PNP0800 cihazları olarak tanımlanmayan) algılayıp kullanmaya çalışır, ancak sistem hoparlörü seçeneğini yalnızca donanım düzeyinde erişilebilirse etkinleştirebilir. Kullanılabilir bir çıkış bulunamazsa, normal ses cihazınızı kullanmanız gerekecektir.

## 2.1 Sistem Hoparlörü Çıkış Testi (Ultrasonik Frekans Algılama)
  NeoBleeper, cihaz Windows tarafından bildirilmese bile (PNP0800 yerine PNP0C02 gibi belirli kimliklerle), sistem hoparlörü (diğer adıyla PC hoparlörü) çıkışını algılamak için yeni ve gelişmiş bir donanım testi içeriyor. Bu test, ultrasonik frekansları (genellikle duyulamayan 30–38 kHz) kullanır ve sistem hoparlörü bağlantı noktasındaki elektriksel geri bildirimi analiz eder.

- **Nasıl çalışır:**
  Başlangıç ​​sırasında NeoBleeper, normal cihaz kimliği kontrolünden sonra ikinci bir adım gerçekleştirir. Sistem hoparlörü bağlantı noktasına ultrasonik sinyaller gönderir ve gizli veya standart dışı olsa bile işlevsel bir hoparlör çıkışının varlığını algılamak için donanım geri bildirimini izler.

- **Neleri fark edebilirsiniz:**
  Bazı sistemlerde, özellikle piezo buzzer bulunan kısmında, bu aşamada hafif tıklama sesleri duyabilirsiniz. Bu normaldir ve donanım testinin çalıştığını gösterir.

  ![image4](https://github.com/user-attachments/assets/7c205527-8fe5-4ff4-9772-b8da11f2c521)
  
  *Sistem hoparlörü (PC hoparlörü) çıkışı varlığı 2/2. adımda kontrol ediliyor... (tıklama sesleri duyabilirsiniz)*

- **Bu test neden yapılmaktadır?**
  Birçok modern sistemde PNP0800 sistem hoparlörü aygıtı bulunmaz, ancak yine de kullanılabilir (gizli) bir hoparlör çıkışı vardır. NeoBleeper, daha fazla donanımda bip sesi özelliklerini etkinleştirmek için bu gelişmiş yöntemi kullanır.

---

## 3. ARM64 Desteği ve Sınırlamaları

**ARM64 tabanlı cihazlar:**

Windows ARM64 sistemlerinde, "Sistem Hoparlörü" testi ve "Bip sesi oluşturmak için ses aygıtını kullan" onay kutusu NeoBleeper'da **mevcut değildir**. Bunun yerine, tüm bip sesleri ve ses çıkışları her zaman standart ses aygıtınız (hoparlörler veya kulaklıklar) aracılığıyla üretilir.

- "Sistem Hoparlörünü Test Et" düğmesi ve ilgili algılama özellikleri, ARM64 cihazlarındaki ayarlarda **görünmez**.
- "Bip sesi oluşturmak için ses aygıtını kullan" seçeneği mevcut değildir çünkü bu davranış otomatik olarak uygulanır.
- Bu sınırlama, ARM64 Windows platformlarında PC/sistem hoparlör donanımına doğrudan erişim sağlanamadığı için mevcuttur.
- ARM64'te her zaman normal ses çıkış aygıtınızdan bip sesleri duyarsınız.

**Bir ARM64 bilgisayar kullanıyorsanız ve NeoBleeper'da sistem hoparlörü seçeneklerini görmüyorsanız, bu beklenen bir durumdur ve bir hata değildir.**

---

## 4. Sistem Hoparlörü Varlığını Kontrol Etme

- **Masaüstü bilgisayarlar:** Eski masaüstü bilgisayarların çoğunda anakart üzerinde bir PC hoparlörü başlığı bulunur. Daha yeni sistemlerde bu özellik bulunmayabilir veya çıkış, NeoBleeper'ın artık kullanabileceği gizli/PNP0800 olmayan bir biçimde sunulabilir.
- **Dizüstü bilgisayarlar:** Çoğu dizüstü bilgisayarda ayrı bir sistem hoparlörü bulunmaz; tüm ses ana ses sistemi üzerinden yönlendirilir.
- **Sanal makineler:** Sistem hoparlörü emülasyonu genellikle yoktur veya güvenilir değildir; PNP0800 olmayan çıkışlar kullanılamayabilir.
- **Nasıl anlaşılır:** Yukarıdaki uyarıları görüyorsanız ancak NeoBleeper'da sistem hoparlörünü etkinleştirip test edebiliyorsanız, bilgisayarınızda muhtemelen gizli veya standart dışı bir çıkış vardır.
  
---

## 5. Hiç Ses Duyamıyorum!

- **NeoBleeper ayarlarınızı kontrol ediniz:**
  Sistem hoparlörünüz kullanılamıyorsa, ses cihazınızın (hoparlörler/kulaklıklar) doğru şekilde seçildiğinden ve çalıştığından emin olunuz.
- **Windows ses karıştırıcısını kontrol ediniz:**
  NeoBleeper'ın sistem ses karıştırıcısında sessize alınmadığından emin olunuz.
- **"Sistem Hoparlörünü Test Et" düğmesini deneyiniz:**
  Bilgisayarınızın hoparlörünü test etmek için kullanınız.
- **Uyarı mesajlarını okuyunuz:**
  NeoBleeper, sistem hoparlörünüze erişemezse özel talimatlar verecektir.

---

## 6. Yapay Zeka Uyarıları, Hatalar ve Google Gemini™ API Sorun Giderme

NeoBleeper'ın "Yapay Zeka ile Müzik Oluştur" özelliği, Google Gemini™ API'sini kullanır. API kullanılabilirliği, kullanım sınırları veya ülke kısıtlamalarıyla ilgili belirli hata iletişim kutuları veya uyarılarla karşılaşabilirsiniz.

### 6.1 Kota veya Oran Sınırı Hataları (429 RESOURCE_EXHAUSTED)

![image1](https://github.com/user-attachments/assets/29f073fa-b5fa-41a5-ae1b-26d78d0a9aad)

**Olası Nedenler:**
- **Hesabınızın API kotası tükendi.** Ücretsiz bir API anahtarı kullanıyorsanız, bazı modeller (örneğin `gemini-2.0-pro-exp`) kullanılamayabilir veya ücretsiz hesaplar için çok düşük/kesin kullanım sınırları olabilir. - **Ücretsiz katman sınırlamaları:** Bazı yeni jenerasyon modelleri (Gemini Pro Exp gibi) ücretsiz katman kullanıcıları tarafından kullanılamaz. Bunları kullanmaya çalışmak kota veya kullanılabilirlik hatasıyla sonuçlanır.
- **Hız sınırlarının aşılması:** Çok fazla isteği çok hızlı gönderirseniz, ücretli bir planda bile API'nin hız sınırlarına ulaşabilirsiniz.

**Nasıl Düzeltilir:**
- **API kotanızı ve faturalandırmanızı kontrol edin:** Kullanımınızı doğrulamak ve gerekirse planınızı yükseltmek için Google Cloud/Gemini hesabınıza giriş yapınız.
- **Yalnızca desteklenen modelleri kullanın:** Ücretsiz katman kullanıcıları belirli modellerle sınırlı olabilir. Kullanılabilir modeller için belgeleri kontrol edin veya desteklenen bir modele geçiniz.
- **Bekleyin ve daha sonra tekrar deneyin:** Bazen, birkaç dakika beklemek, mesajın geri sayımında belirtildiği gibi kotanın geçici olarak yenilenmesine olanak tanır.
- **Güncel kullanım politikaları ve hız sınırları için [Gemini API belgelerini](https://ai.google.dev/gemini-api/docs/rate-limits) inceleyiniz.**

---

### 6.2 Çok Yeni veya Belgelenmemiş Gemini Modelleri için Sorun Giderme (örneğin, Gemini 3 Pro Preview)

Bazı Gemini modelleri, özellikle de **Gemini 3 Pro Preview** gibi yepyeni sürümler, lansman sırasında resmi Gemini API fiyatlandırma veya kota belgelerinde görünmeyebilir. Genel hesap kotanız kullanılmamış gibi görünse bile kota, erişim veya "RESOURCE_EXHAUSTED" hatalarıyla karşılaşabilirsiniz.

**Çok yeni modeller için önemli hususlar:**
- Google, önizleme modellerine (Gemini 3 Pro Preview gibi) erişimi genellikle belirli hesaplarla veya belirli bölgelerle sınırlar ve çok daha katı istek ve kullanım sınırları uygulayabilir.
- Ücretsiz hesaplarda bu modeller için kota sıfır olabilir veya istekler tamamen engellenebilir.
- Model, yayınlandıktan sonraki birkaç hafta boyunca kota/fiyatlandırma sekmelerinde veya Google belgelerinde görünmeyebilir.
- Yeni Gemini modellerinin fiyatlandırması, erişimi ve kullanılabilirliği sık sık değişebilir.

**Hatalarla karşılaşırsanız ne yapmalısınız?**
- [API kullanımınızı ve kotalarınızı](https://ai.dev/usage?tab=rate-limit) ve yeni modelin konsolunuzda görünüp görünmediğini tekrar kontrol ediniz.
- [Gemini API belgelerini](https://ai.google.dev/gemini-api/docs/rate-limits) inceleyin, ancak belgelerin yeni yayınlanan modellerin gerisinde kalabileceğini unutmayınız.
- Resmi fiyatlandırma tablolarında belgelenmemiş bir model için "RESOURCE_EXHAUSTED" gibi hatalar görüyorsanız, bu muhtemelen modelin henüz genel kullanıma sunulmadığı veya çok kısıtlı önizleme erişimine sahip olduğu anlamına gelir.
- Bu deneysel modelleri kullanmanız gerekiyorsa, Google'ın belgelerini güncellemesini ve daha geniş bir kullanıma sunulmasını bekleyiniz.

> **Not:**
> NeoBleeper ve benzeri uygulamalar bu sınırlamaları aşamaz. Hesabınız veya bölgeniz uygun değilse, Google'ın seçtiğiniz Gemini modeli için erişimi resmi olarak etkinleştirmesini veya kotayı artırmasını beklemelisiniz.

---

### 6.3 Bölge veya Ülke Kısıtlamaları

#### "API ülkenizde mevcut değil"

![image4](https://github.com/user-attachments/assets/e3223ed6-9ad5-4cf5-b405-d80e64dd2dd9)

Bölgesel veya yasal kısıtlamalar nedeniyle bazı bölgeler Google Gemini™ API için desteklenmemektedir.

**Olası Nedenler:**
- Ülkeniz, Gemini API kullanılabilirliğinin kısıtlı olduğu bir ülkedir.
- Kullandığınız API anahtarı, erişimi olmayan bir bölgeye kayıtlıdır.

**Nasıl Düzeltilir:**
- Resmi belgelerde **Google Gemini™ API'nin desteklediği ülkeleri** kontrol ediniz. - Kısıtlı bir ülkedeyseniz, yapay zeka özellikleri kullanılamayacaktır.

#### Bölgeye Özel Uyarı (Ayarlar Paneli)

![image3](https://github.com/user-attachments/assets/a5b31462-fb27-496a-aea3-12eca84a43db)

Avrupa Ekonomik Alanı, İsviçre veya Birleşik Krallık'ta, Google Gemini™ API ücretli (ücretsiz olmayan) bir hesap gerektirebilir.

- Bu uyarıyı görürseniz, yapay zeka özelliklerini kullanmaya başlamadan önce Gemini API planınızı yükselttiğinizden emin olunuz.

---

### 6.4 Genel Yapay Zeka API Tavsiyeleri

- Yalnızca kendi API anahtarınızı giriniz; güvenliğiniz için paylaşmayınız.
- NeoBleeper, API anahtarınızı, özellik kullanımı için ihtiyaç duyulması dışında doğrudan Gemini hizmetine iletmez. - Tekrarlayan hatalarla karşılaşırsanız, API anahtarınızı kaldırıp yeniden eklemeyi deneyin ve anahtarınızın etkin olduğundan emin olun.

---

## 7. Belirli Yonga Setleri için Sistem Hoparlörü ve Ses Tavsiyeleri (Intel B660 dahil)

### Ses duymuyorsanız, ses bozuksa veya sistem hoparlörü güvenilir değilse:

Intel B660 serisi ve daha yenileri de dahil olmak üzere bazı modern yonga setlerinde, sistem hoparlörünün (PC bip sesi) başlatılması veya yeniden başlatılmasıyla ilgili sorunlar olabilir ve bu da sessizliğe veya ses sorunlarına neden olabilir.

**Etkilenen kullanıcılar için tavsiyeler:**

- **Bilgisayarınızı uyku moduna alıp tekrar uyandırmayı deneyin.**

Bu, sistem hoparlöründen sorumlu düşük seviyeli donanım bağlantı noktasını yeniden başlatmanıza veya sıfırlamanıza ve bip sesi işlevini geri yüklemenize yardımcı olabilir.
- Sistem hoparlörü çıkışı güvenilir değilse, **geri dönüş olarak "Bip sesi oluşturmak için ses aygıtını kullan" özelliğini kullanınız.**
- **BIOS veya donanım yazılımı güncellemelerini kontrol edin:** Bazı anakart üreticileri, hoparlör bağlantı noktası uyumluluğunu iyileştiren güncellemeler yayınlayabilir.

- **Masaüstüne özel:** Sistem hoparlörü donanımı eklediyseniz, çıkardıysanız veya yeniden bağladıysanız, tam bir güç döngüsü gerçekleştiriniz.

_Bu geçici çözüm ayarlar bölümünde vurgulanmıştır:_

![image2](https://github.com/user-attachments/assets/f6c107e0-d617-49e1-be1e-b24859eda282)

> *Ses duymuyorsanız veya ses bozuksa, bilgisayarınızı uyku moduna alıp uyandırmayı deneyin. Bu, etkilenen yonga setlerinde sistem hoparlörünün yeniden başlatılmasına yardımcı olabilir.*

---

*Burada ele alınmayan herhangi bir ses veya yapay zeka sorunu için, lütfen destek talep ederken veya bir GitHub sorunu açarken hata ekran görüntülerini, bilgisayar donanımınızın ayrıntılarını (özellikle anakart/yonga seti marka ve modeli) ve ülkenizi/bölgenizi ekleyin.*

---

## 8. Sıkça Sorulan Sorular

### S: Donanımımda PNP0800 aygıtı yoksa sistem hoparlörünü kullanabilir miyim?
**C:** Evet! NeoBleeper artık mümkün olan yerlerde gizli veya PNP0800 olmayan sistem hoparlör çıkışlarını algılayıp kullanmaya çalışıyor. Başarılı olursa, Windows standart bir aygıt bildirmese bile sistem hoparlörünü kullanabilirsiniz.

### S: "Bip sesi oluşturmak için ses aygıtını kullan" ayarı neden bazen kalıcı hale geliyor (eski sürümlerde)?
**C:** Standart bir sistem hoparlörü çıkışı algılanmadığında (eski sürümlerde), NeoBleeper ses çıkışının hala mümkün olmasını sağlamak için bu ayarı zorunlu kılar.

### S: Eksik sistem hoparlörü için bir çözüm var mı?
**C:** Standart bir sistem hoparlörü çıkışı bulunamazsa (eski sürümlerde), normal ses aygıtınızı (hoparlörler/kulaklıklar) kullanmanız gerekir.

### S: Bip Sesi Durdurucu aracı takılı kalan bip sesini durdurmazsa ne olur?
**C:** Bip Sesi Durdurucu yardımcı programı başarısız olursa, hoparlör donanımını sıfırlamak için bilgisayarınızı yeniden başlatınız.

### S: Başlatma sırasında neden tıklama sesleri duyuyorum?
**C:** Gelişmiş sistem hoparlörü çıkış testi sırasında (2. adım), NeoBleeper gizli veya standart dışı hoparlör çıkışlarını tespit etmek için donanıma ultrasonik sinyaller gönderir. Bazı sistemlerde (özellikle piezo buzzer bulunan kısmında) bu durum hafif tıklama seslerine neden olabilir. Bu normaldir ve bir sorun olduğunu göstermez; yalnızca donanım testinin çalıştığı anlamına gelir.

### S: Ultrasonik donanım testi (2. adım) bozuk (açık devre) veya bağlantısı kesilmiş sistem hoparlörlerini tespit edebilir mi?
**C:** Bu şu anda test edilmemiştir ve bilinmemektedir. Test, elektriksel geri beslemeyi ve port etkinliğini kontrol etse de, fiziksel olarak mevcut ancak bozuk (açık devre) veya bağlantısı kesilmiş bir hoparlör ile eksik bir hoparlör arasında güvenilir bir şekilde ayrım yapamayabilir. Hoparlör tamamen bozuk veya bağlantısı kesilmişse (açık devre), test negatif olarak döndürebilir ve işlevsel bir çıkış algılanmadığını gösterebilir. Ancak, bu davranış garanti edilmez ve belirli donanıma ve arıza moduna bağlı olabilir. Sistem hoparlörünüzün çalışmadığından şüpheleniyorsanız, fiziksel inceleme yapmanız veya bir multimetre kullanmanız önerilir.

### S: ARM64 cihazımda neden sistem hoparlörü veya bip sesi seçenekleri göremiyorum?
**C:** Windows ARM64 sistemlerinde, NeoBleeper sistem hoparlörü ile ilgili ayarları devre dışı bırakır çünkü ARM64 platformları doğrudan sistem hoparlörü donanım erişimini desteklemez. Tüm bip sesleri normal ses çıkış cihazınızdan (hoparlörler veya kulaklıklar) çalınır ve "Sistem Hoparlörünü Test Et" ve "Bip sesi oluşturmak için ses cihazını kullan" seçenekleri otomatik olarak gizlenir. Bu davranış tasarım gereğidir ve bir hata değildir.

### S: "Standart dışı sistem hoparlörü çıkışı mevcut" uyarısı aldığımda bu ne anlama geliyor?
**C:** NeoBleeper, geleneksel PC hoparlör standartlarına uymayan hoparlör donanımı tespit etti (örneğin, bir PNP0800 aygıtı değil). Bu, modern masaüstlerinde veya sanal makinelerde bulunan "gizli" bir hoparlör çıkışı olabilir. Bu durumlarda, tüm bip sesi özellikleri güvenilir bir şekilde çalışmayabilir, ancak NeoBleeper tespit edebildiği herhangi bir uyumlu çıkışı kullanmaya çalışacaktır.

### S: Windows bir PC hoparlörü aygıtı listelemese bile "Sistem Hoparlörünü Test Et" düğmesi neden mevcut?
**C:** NeoBleeper, gizli veya standart dışı sistem hoparlör çıkışları için algılama mantığı içerir. Düğme görünüyorsa, Windows tarafından bir aygıt olarak bildirilmemiş olsa bile NeoBleeper'ın hoparlör çıkışı için olası bir donanım bağlantı noktası bulduğu anlamına gelir.

### S: Yapay Zeka özellikleri için Google Gemini™ API'sini kullanıyorum ve "kota tükendi" veya "API ülkenizde kullanılamıyor" mesajını görüyorum. Ne yapmalıyım?
**C:** Bu kılavuzun 6. bölümüne bakın. API anahtarınızın ve faturalandırma/kotanızın geçerli olduğundan ve kullanımınızın Google'ın bölgesel kısıtlamalarına uygun olduğundan emin olunuz. Kısıtlı bir bölgedeyseniz, maalesef yapay zeka özellikleri kullanılamayabilir.

### S: Intel B660 (veya daha yeni) bir sistemim var ve PC hoparlörüm bazen çalışmıyor veya takılıyor. Bu normal mi?
**C:** Bazı yeni yonga setlerinde, sistem hoparlörünü yeniden başlatmayla ilgili bilinen uyumluluk sorunları vardır. Bilgisayarınızı uyku moduna alıp uyandırmayı deneyin veya normal ses cihazınızı kullanınız. Hoparlör desteğini iyileştirebilecek BIOS/donanım güncellemelerini kontrol edin.

### S: Destek için ses veya yapay zeka sorunlarını bildirmenin en iyi yolu nedir?
**C:** Mümkün olduğunca fazla bilgi sağlayın: bilgisayarınızın markası/modeli, bölgesi, hata iletişim kutularının ekran görüntüleri ve NeoBleeper klasöründeki `DebugLog.txt` dosyanız. Yapay zeka sorunları için, hata iletişim kutularının tam metnini ekleyin ve Gemini API hesap durumunuzu açıklayın.

### S: Bir çökme veya zorla kapatma sonrasında NeoBleeper'ın bip sesi durdurucusu sürekli bip sesini durdurmadı. Bunu düzeltmenin başka bir yolu var mı?
**C:** Beep Stopper etkisizse, bilgisayarınızı yeniden başlatmak sistem hoparlörü donanımını sıfırlayacak ve sürekli bip sesini durduracaktır.

### S: Standart dışı veya eksik sistem hoparlör çıkışı hakkında bir uyarı mesajı görürsem Beep Stopper yardımcı programını kullanmak güvenli midir?
**C:** Evet, ancak yardımcı programın donanımı kontrol edemeyebileceğini ve nadir durumlarda kararsızlığa veya hiçbir etki göstermemesine neden olabileceğini unutmayınız. Emin değilseniz, devam etmemeyi ve bilgisayarınızı yeniden başlatmayı seçiniz.

### S: Sanal makinelerde sistem hoparlörünü hiç çalıştıramıyorum. Bu bir hata mı?
**C:** Kesinlikle değil. Birçok sanal makine, bir bilgisayar hoparlörünü düzgün bir şekilde taklit etmez veya çıkışı programlanabilir bir şekilde kontrol edilemeyecek şekilde sunar. En iyi sonuçlar için standart ses çıkış cihazınızı kullanın.

**Potansiyel gelecekteki güncellemeler:**
Gelecekteki testler veya geliştirmeler, NeoBleeper'ın ultrasonik donanım testi aracılığıyla bozuk veya bağlantısı kesilmiş sistem hoparlörlerini güvenilir bir şekilde tespit etmesini sağlarsa, bu SSS ve tespit mantığı bu geliştirmeleri yansıtacak şekilde güncellenecektir. Ayrıntılar için değişiklik günlüklerini veya yeni sürümleri takip ediniz.

---

## 9. Yardım Alma

- **Bilgisayar ve ortam ayrıntılarını sağlayınız:** Donanım algılama veya ses sorunlarını bildirirken, lütfen bilgisayarınız (masaüstü/dizüstü bilgisayar, üretici/model, işletim sistemi) ve ilgili donanımlar hakkında ayrıntıları ekleyiniz.
- **Ekran görüntüleri veya hata iletişim kutuları ekleyiniz:** Hata veya uyarı iletişim kutularının ekran görüntüleri çok faydalıdır. Sorunun tam olarak ne zaman ortaya çıktığını belirtiniz.
- **Günlük dosyasını ekleyiniz:** Yeni sürümlerden başlayarak, NeoBleeper program klasöründe `DebugLog.txt` adlı ayrıntılı bir günlük dosyası oluşturur. Yardım isterken lütfen bu dosyayı ekleyiniz, çünkü yararlı tanılama bilgileri içerir.
- **Sorunu yeniden oluşturma adımlarını açıklayınız:** Sorun oluştuğunda ne yaptığınızı açıkça belirtiniz.
- **GitHub'da bir sorun açınız:** Daha fazla yardım için, GitHub'da bir sorun açın ve en iyi destek için yukarıdaki tüm ayrıntıları ekleyiniz.

_Bu kılavuz, yeni sorunlar ve çözümler keşfedildikçe güncellenir. Daha fazla yardım için lütfen kurulumunuz ve karşılaştığınız sorun hakkında ayrıntılı bilgi içeren bir GitHub sorunu açınız._
