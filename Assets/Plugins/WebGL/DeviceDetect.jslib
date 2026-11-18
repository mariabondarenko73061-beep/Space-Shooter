/*
 * Определяет, является ли основное устройство ввода сенсорным (телефон/планшет).
 * Нужен потому, что в WebGL Application.isMobilePlatform всегда возвращает false,
 * даже когда игра открыта на телефоне.
 */
mergeInto(LibraryManager.library, {
  // Возвращает 1 для сенсорного устройства, иначе 0.
  SS_IsTouchPrimaryDevice: function () {
    try {
      // Основной указатель "грубый" (палец) — типичный признак телефона/планшета.
      if (window.matchMedia && window.matchMedia('(pointer: coarse)').matches) {
        return 1;
      }
    } catch (e) { }

    try {
      var ua = navigator.userAgent || '';
      var mobileUA = /Android|iPhone|iPad|iPod|IEMobile|Opera Mini|Mobile|Tablet/i.test(ua);
      if (mobileUA && navigator.maxTouchPoints > 0) {
        return 1;
      }
    } catch (e) { }

    return 0;
  }
});
