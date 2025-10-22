# NeoBleeper Sorun Giderme Rehber,

Bu kılavuz, NeoBleeper programını kullanırken karşılaşılan yaygın sorunlara, özellikle sistem hoparlör davranışı, ses çıkışı, donanım uyumluluğu ve sürekli sistem bip sesi ile ilgili sorunlara yönelik çözümler sağlar.

---

## 1. Çökme veya Zorla Kapatma Sonrasında Sistem Hoparlöründe Ses Takılması

**Sorun:**
NeoBleeper, sistem (PC) hoparlöründen ses çalarken çökerse veya zorla kapatılırsa, ses "takılabilir" ve bu da sürekli bip sesi veya vızıltıya neden olabilir.

**Neden Olur?:**
Sistem hoparlörü düşük donanım/yazılım seviyesinde kontrol edilir. Uygulama çıkışta hoparlörü düzgün bir şekilde serbest bırakmazsa veya sıfırlanmazsa ses devam edebilir.

**Çözümler:**
- **NeoBleeper Bip Sesi Durdurucu yardımcı programını kullanın:**
  NeoBleeper, program klasöründe "NeoBleeper Bip Sesi Durdurucu" adlı bir araç ile birlikte gelir.

  ![image4](https://github.com/user-attachments/assets/b36256ad-916d-42ab-83ea-3271dd897ce1)
  
  - Bu aracı çalıştırın ve sistem hoparlöründen gelen takılıp kalmış bip sesini durdurmak için **Bip Sesini Durdur** düğmesine basınız.
  - Bu aracı yalnızca bir çökme veya zorla çıkıştan sonra bip sesi devam ettiğinde kullanınız.

  #### Bip Durdurucu Mesajları ve Anlamları

  Bip Sesi Durdurucu yardımcı programını kullandığınızda aşağıdaki mesajları görebilirsiniz:

  ![image1](https://github.com/user-attachments/assets/e8a11d67-3a9c-424b-9eae-e50258408b98)
    
    **Sistem hoparlörü bip sesi çıkarmıyor veya sistem hoparlörü farklı bir şekilde bip sesi çıkarıyor. Herhangi bir işlem yapılmadı.**  
    Bu mesaj, yardımcı program sistem hoparlörünü kontrol ettiğinde ve hoparlörün bip sesi çıkarmadığını veya araç tarafından kontrol edilemeyen bir şekilde bip sesi çıkardığını tespit ettiğinde görüntülenir. Bu durumda, Bip Durdurucu başka bir işlem yapmaz.
    - *İpucu:* Hala sürekli bir bip sesi duyuyorsanız, bilgisayarınızı yeniden başlatmayı deneyin.
      
  ![image2](https://github.com/user-attachments/assets/a0064c46-b4b3-430d-bce6-36fa1c754685)
    
    **Bip sesi başarıyla durduruldu!**  
    Bu ileti, Bip Sesi DUrdurucu yardımcı programının takılı kalmış bir bip sesi algıladığını ve bunu başarıyla durdurabildiğini onaylar. Başka bir işlem yapmanıza gerek yoktur.

  ![image3](https://github.com/user-attachments/assets/7c00d628-440a-414d-a1f1-b7860f9a061b)
  
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

![image2](https://github.com/user-attachments/assets/93c1144f-0201-4599-b892-b0f272456d07)

![image3](https://github.com/user-attachments/assets/5699b1b2-93d1-4f67-b5c1-319aabaccf0a)


- **"Sistem Hoparlörünü Test Et" düğmesinin kullanılabilirliği:**
  NeoBleeper, gizli veya PNP0800 olmayan çıkışlar da dahil olmak üzere kullanılabilir herhangi bir sistem hoparlörü çıkışı algılarsa bu seçenek etkinleştirilir.

- **"Bip sesi oluşturmak için ses cihazını kullan" ayarı:**
  Artık gizli veya standart dışı bir sistem hoparlörü çıkışı algılanırsa bu özelliği devre dışı bırakabilirsiniz.

#### "Standart dışı sistem hoparlörü çıkışı" ne anlama geliyor?
Bazı modern bilgisayarlarda, dizüstü bilgisayarlarda veya sanal makinelerde gerçek bir PC hoparlörü bulunmaz veya sinyal yönlendirmesi standart dışıdır. NeoBleeper artık bu tür gizli sistem hoparlörü çıkışlarını (PNP0800 cihazları olarak tanımlanmayan) algılayıp kullanmaya çalışır, ancak sistem hoparlörü seçeneğini yalnızca donanım düzeyinde erişilebilirse etkinleştirebilir. Kullanılabilir bir çıkış bulunamazsa, normal ses cihazınızı kullanmanız gerekecektir.

---

## 3. Sistem Hoparlörü Varlığını Kontrol Etme

- **Masaüstü bilgisayarlar:** Eski masaüstü bilgisayarların çoğunda anakart üzerinde bir PC hoparlörü başlığı bulunur. Daha yeni sistemlerde bu özellik bulunmayabilir veya çıkış, NeoBleeper'ın artık kullanabileceği gizli/PNP0800 olmayan bir biçimde sunulabilir.
- **Dizüstü bilgisayarlar:** Çoğu dizüstü bilgisayarda ayrı bir sistem hoparlörü bulunmaz; tüm ses ana ses sistemi üzerinden yönlendirilir.
- **Sanal makineler:** Sistem hoparlörü emülasyonu genellikle yoktur veya güvenilir değildir; PNP0800 olmayan çıkışlar kullanılamayabilir.
- **Nasıl anlaşılır:** Yukarıdaki uyarıları görüyorsanız ancak NeoBleeper'da sistem hoparlörünü etkinleştirip test edebiliyorsanız, bilgisayarınızda muhtemelen gizli veya standart dışı bir çıkış vardır.
  
---

## 2.1 Sistem Hoparlör Çıkış Testi (Ultrasonik Frekans Algılama)
  NeoBleeper, cihaz Windows tarafından bildirilmese bile (PNP0800 yerine PNP0C02 gibi belirli kimliklerle), sistem hoparlörü (diğer adıyla PC hoparlörü) çıkışını algılamak için yeni ve gelişmiş bir donanım testi içeriyor. Bu test, ultrasonik frekansları (genellikle duyulamayan 30–38 kHz) kullanır ve sistem hoparlörü bağlantı noktasındaki elektriksel geri bildirimi analiz eder.

- **Nasıl çalışır:**
  Başlangıç ​​sırasında NeoBleeper, normal cihaz kimliği kontrolünden sonra ikinci bir adım gerçekleştirir. Sistem hoparlörü bağlantı noktasına ultrasonik sinyaller gönderir ve gizli veya standart dışı olsa bile işlevsel bir hoparlör çıkışının varlığını algılamak için donanım geri bildirimini izler.

- **Neleri fark edebilirsiniz:**
  Bazı sistemlerde, özellikle piezo buzzer bulunan kısmında, bu aşamada hafif tıklama sesleri duyabilirsiniz. Bu normaldir ve donanım testinin çalıştığını gösterir.

  ![image4](https://github.com/user-attachments/assets/d3e35c02-9e66-464d-84cd-c9d94955f248)
  
  *Sistem hoparlörü (PC hoparlörü) çıkışı varlığı 2/2. adımda kontrol ediliyor... (tıklama sesleri duyabilirsiniz)*

- **Bu test neden?**
  Birçok modern sistemde PNP0800 sistem hoparlörü aygıtı bulunmaz, ancak yine de kullanılabilir (gizli) bir hoparlör çıkışı vardır. NeoBleeper, daha fazla donanımda bip sesi özelliklerini etkinleştirmek için bu gelişmiş yöntemi kullanır.

## 4. Hiç Ses Duyamıyorum!

- **NeoBleeper ayarlarınızı kontrol ediniz:**
  Sistem hoparlörünüz kullanılamıyorsa, ses cihazınızın (hoparlörler/kulaklıklar) doğru şekilde seçildiğinden ve çalıştığından emin olunuz.
- **Windows ses karıştırıcısını kontrol ediniz:**
  NeoBleeper'ın sistem ses karıştırıcısında sessize alınmadığından emin olunuz.
- **"Sistem Hoparlörünü Test Et" düğmesini deneyiniz:**
  Bilgisayarınızın hoparlörünü test etmek için kullanınız.
- **Uyarı mesajlarını okuyunuz:**
  NeoBleeper, sistem hoparlörünüze erişemezse özel talimatlar verecektir.

---

## 5. Sıkça Sorulan Sorular

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

**Potansiyel gelecekteki güncellemeler:**
Gelecekteki testler veya geliştirmeler, NeoBleeper'ın ultrasonik donanım testi aracılığıyla bozuk veya bağlantısı kesilmiş sistem hoparlörlerini güvenilir bir şekilde tespit etmesini sağlarsa, bu SSS ve tespit mantığı bu geliştirmeleri yansıtacak şekilde güncellenecektir. Ayrıntılar için değişiklik günlüklerini veya yeni sürümleri takip ediniz.

---

## 6. Yardım Alma

- **Bilgisayar ve ortam ayrıntılarını sağlayınız:** Donanım algılama veya ses sorunlarını bildirirken, lütfen bilgisayarınız (masaüstü/dizüstü bilgisayar, üretici/model, işletim sistemi) ve ilgili donanımlar hakkında ayrıntıları ekleyiniz.
- **Ekran görüntüleri veya hata iletişim kutuları ekleyiniz:** Hata veya uyarı iletişim kutularının ekran görüntüleri çok faydalıdır. Sorunun tam olarak ne zaman ortaya çıktığını belirtiniz.
- **Günlük dosyasını ekleyiniz:** Yeni sürümlerden başlayarak, NeoBleeper program klasöründe `DebugLog.txt` adlı ayrıntılı bir günlük dosyası oluşturur. Yardım isterken lütfen bu dosyayı ekleyiniz, çünkü yararlı tanılama bilgileri içerir.
- **Sorunu yeniden oluşturma adımlarını açıklayınız:** Sorun oluştuğunda ne yaptığınızı açıkça belirtiniz.
- **GitHub'da bir sorun açınız:** Daha fazla yardım için, GitHub'da bir sorun açın ve en iyi destek için yukarıdaki tüm ayrıntıları ekleyiniz.

_Bu kılavuz, yeni sorunlar ve çözümler keşfedildikçe güncellenir. Daha fazla yardım için lütfen kurulumunuz ve karşılaştığınız sorun hakkında ayrıntılı bilgi içeren bir GitHub sorunu açınız._
