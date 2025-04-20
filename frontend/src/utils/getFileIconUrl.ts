export function getFileIconUrl(fileName: string, size: number): string {
    const ext = fileName.split('.').pop()?.toLowerCase() || '';

    const fileTypes: { [category: string]: string[] } = {
        powerpoint: ['ppt', 'pptx'],
        word: ['doc', 'docx', 'dotx', 'rtf'],
        archive: ['7z', 'gz', 'rar', 'tar', 'zip'],
        video: ['avi', 'mkv', 'mov', 'mp4', 'webm'],
        image: ['bmp', 'gif', 'jpeg', 'jpg', 'png', 'webp'],
        code: ['c', 'cpp', 'cs', 'go', 'java', 'js', 'php', 'py', 'rb', 'rs', 'swift', 'ts'],
        excel: ['csv', 'xls', 'xlsx'],
        audio: ['flac', 'mp3', 'ogg', 'wav'],
        text: ['md', 'txt'],
        pdf: ['pdf']
    };

    const categoryUrls: { [category: string]: string } = {
        powerpoint: `https://ssl.gstatic.com/docs/doclist/images/mediatype/icon_1_powerpoint_x${size}.png`,
        word: `https://ssl.gstatic.com/docs/doclist/images/mediatype/icon_1_word_x${size}.png`,
        archive: `https://ssl.gstatic.com/docs/doclist/images/mediatype/icon_2_archive_x${size}.png`,
        video: `https://ssl.gstatic.com/docs/doclist/images/mediatype/icon_1_video_x${size}.png`,
        image: `https://ssl.gstatic.com/docs/doclist/images/mediatype/icon_1_image_x${size}.png`,
        code: `https://ssl.gstatic.com/docs/doclist/images/mediatype/icon_3_code_x${size}.png`,
        excel: `https://ssl.gstatic.com/docs/doclist/images/mediatype/icon_1_excel_x${size}.png`,
        audio: `https://ssl.gstatic.com/docs/doclist/images/mediatype/icon_1_audio_x${size}.png`,
        text: `https://ssl.gstatic.com/docs/doclist/images/mediatype/icon_1_text_x${size}.png`,
        pdf: `https://drive-thirdparty.googleusercontent.com/${size}/type/application/pdf`
    };

    for (const [category, extensions] of Object.entries(fileTypes)) {
        if (extensions.includes(ext)) {
            return categoryUrls[category];
        }
    }

    return `https://drive-thirdparty.googleusercontent.com/${size}/type/application/octet-stream`;
}