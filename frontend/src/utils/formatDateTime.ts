import {format} from 'date-fns/format';

export const formatDate = (date: string) => format(new Date(date).toLocaleDateString(), 'dd.MM.yyyy');

export const formatDateTime = (date: string) => format(new Date(date).toLocaleDateString(), 'dd.MM.yyyy HH:mm');