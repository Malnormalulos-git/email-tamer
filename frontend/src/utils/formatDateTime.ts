export const formatDateTime = (date: string) => {
    const utcDate = new Date(date.endsWith('Z') ? date : date + 'Z');

    const pad = (num: number) => num.toString().padStart(2, '0');

    const day = pad(utcDate.getDate());
    const month = pad(utcDate.getMonth() + 1);
    const year = utcDate.getFullYear();
    const hours = pad(utcDate.getHours());
    const minutes = pad(utcDate.getMinutes());

    return `${day}.${month}.${year} ${hours}:${minutes}`;
};
